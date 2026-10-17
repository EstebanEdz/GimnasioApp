using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

/// <summary>
/// Ciclo de vida de una membresia: Vigente -> (Congelada) -> Vencida -> Renovada / Cancelada.
/// Cada cambio exige permiso, valida el estado actual y deja rastro en la auditoria.
/// </summary>
public class MembresiaService
{
    private readonly MembresiaDAL _dal = new();
    private readonly InscripcionDAL _inscripcionDal = new();
    private readonly PlanDAL _planDal = new();
    private readonly AuditoriaService _auditoria = new();

    public List<Membresia> Listar(string? estado = null, string? texto = null, int? porVencerEnDias = null, bool soloConSaldo = false)
    {
        SesionActual.Exigir(Permisos.MembresiasVer);
        return _dal.Listar(estado, texto, porVencerEnDias, soloConSaldo).Select(Mapear).ToList();
    }

    /// <summary>Marca como Vencida toda membresia Vigente cuya fecha ya paso. Se ejecuta al iniciar la app.</summary>
    public int ActualizarVencidas()
    {
        int cambiadas = _dal.ActualizarVencidas();
        if (cambiadas > 0)
            _auditoria.Registrar("MEMBRESIAS_VENCIDAS", "Inscripcion", null, $"{cambiadas} membresia(s) pasaron a Vencida");
        return cambiadas;
    }

    public ResumenAlertas ObtenerResumenAlertas()
    {
        int dias = Configuracion.DiasAvisoVencimiento;
        var todas = _dal.Listar(null, null, null, false);
        return new ResumenAlertas
        {
            PorVencer = todas.Count(m => m.EstadoReal == EstadosMembresia.Vigente && m.DiasParaVencer >= 0 && m.DiasParaVencer <= dias),
            Vencidas = todas.Count(m => m.EstadoReal == EstadosMembresia.Vencida),
            ConSaldo = todas.Count(m => m.Saldo > 0 && m.EstadoReal != EstadosMembresia.Cancelada)
        };
    }

    /// <summary>
    /// Renueva creando una inscripcion NUEVA con el mismo plan y entrenador (el historial se conserva).
    /// Si la membresia sigue vigente, la nueva empieza cuando vence la actual (sin perder dias);
    /// si ya vencio, empieza hoy. Devuelve el Id de la nueva inscripcion.
    /// </summary>
    public int Renovar(int inscripcionId)
    {
        SesionActual.Exigir(Permisos.MembresiasGestionar);

        var actual = _dal.ObtenerPorId(inscripcionId) ?? throw new InvalidOperationException("La membresia no existe.");
        if (actual.EstadoReal == EstadosMembresia.Cancelada)
            throw new InvalidOperationException("No se puede renovar una membresia cancelada. Cree una inscripcion nueva.");
        if (actual.EstadoReal == EstadosMembresia.Congelada)
            throw new InvalidOperationException("Reactive la membresia congelada antes de renovarla.");

        var plan = _planDal.ObtenerPorId(actual.PlanId);
        if (plan == null || !plan.Activo)
            throw new InvalidOperationException("El plan de esta membresia ya no esta disponible. Cree una inscripcion con otro plan.");

        var original = _inscripcionDal.ObtenerPorId(inscripcionId);

        DateTime inicio = actual.EstadoReal == EstadosMembresia.Vigente && actual.FechaVencimiento.Date >= DateTime.Today
            ? actual.FechaVencimiento.Date
            : DateTime.Today;

        var nueva = new Inscripcion
        {
            MiembroId = actual.MiembroId,
            PlanId = actual.PlanId,
            EntrenadorId = original?.EntrenadorId,
            FechaInscripcion = inicio,
            Detalle = new List<DetalleInscripcion>
            {
                new DetalleInscripcion { Concepto = $"Plan {plan.Nombre} (renovacion)", Cantidad = 1, PrecioUnitario = plan.Precio }
            }
        };

        int nuevoId = new InscripcionService().Registrar(nueva);
        _auditoria.Registrar("MEMBRESIA_RENOVADA", "Inscripcion", nuevoId,
            $"Renueva la #{inscripcionId} de {actual.Miembro}. Plan {plan.Nombre}, inicia {inicio:yyyy-MM-dd}");
        return nuevoId;
    }

    public void Congelar(int inscripcionId)
    {
        SesionActual.Exigir(Permisos.MembresiasGestionar);
        if (!_dal.Congelar(inscripcionId))
            throw new InvalidOperationException("Solo se puede congelar una membresia vigente que no haya vencido.");
        _auditoria.Registrar("MEMBRESIA_CONGELADA", "Inscripcion", inscripcionId, "Congelada desde hoy");
    }

    /// <summary>Reactiva y alarga la fecha de vencimiento tantos dias como estuvo congelada.</summary>
    public void Descongelar(int inscripcionId)
    {
        SesionActual.Exigir(Permisos.MembresiasGestionar);
        if (!_dal.Descongelar(inscripcionId))
            throw new InvalidOperationException("Solo se puede reactivar una membresia que este congelada.");
        _auditoria.Registrar("MEMBRESIA_REACTIVADA", "Inscripcion", inscripcionId, "Reactivada; vencimiento extendido por los dias congelada");
    }

    public void Cancelar(int inscripcionId, string? motivo)
    {
        SesionActual.Exigir(Permisos.MembresiasGestionar);
        if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 5)
            throw new InvalidOperationException("Escriba el motivo de la cancelacion (minimo 5 caracteres).");

        if (!_dal.Cancelar(inscripcionId))
            throw new InvalidOperationException("Esta membresia ya estaba cancelada o no existe.");
        _auditoria.Registrar("MEMBRESIA_CANCELADA", "Inscripcion", inscripcionId, $"Motivo: {motivo.Trim()}");
    }

    internal static Membresia Mapear(MembresiaRow r) => new Membresia
    {
        InscripcionId = r.InscripcionId,
        MiembroId = r.MiembroId,
        Miembro = r.Miembro,
        Documento = r.Documento,
        PlanId = r.PlanId,
        Plan = r.PlanNombre,
        Entrenador = r.Entrenador,
        FechaInscripcion = r.FechaInscripcion,
        FechaVencimiento = r.FechaVencimiento,
        Estado = r.EstadoReal,
        DiasParaVencer = r.DiasParaVencer,
        Total = r.Total,
        Pagado = r.Pagado,
        Saldo = r.Saldo
    };
}
