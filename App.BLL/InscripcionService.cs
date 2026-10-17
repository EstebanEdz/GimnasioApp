using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class InscripcionService
{
    private readonly InscripcionDAL _dal = new();
    private readonly PlanDAL _planDal = new();
    private readonly PagoDAL _pagoDal = new();
    private readonly AuditoriaService _auditoria = new();

    /// <summary>Crear inscripciones lo puede hacer quien edita inscripciones o quien gestiona membresias (renovaciones).</summary>
    private static void ExigirPuedeInscribir()
    {
        if (!SesionActual.Tiene(Permisos.InscripcionesEditar) && !SesionActual.Tiene(Permisos.MembresiasGestionar))
            throw new UnauthorizedAccessException("Su perfil no tiene permiso para realizar esta accion.");
    }

    /// <summary>
    /// Cola de inscripciones recien registradas que quedan pendientes de
    /// ser atendidas/asignadas por recepcion (requisito Queue&lt;T&gt;: turnos pendientes).
    /// Vive en memoria durante la sesion de la aplicacion.
    /// </summary>
    public Queue<Inscripcion> ColaPendientes { get; } = new();

    /// <summary>
    /// Pila con las ultimas inscripciones eliminadas en la sesion actual,
    /// para poder deshacer la ultima eliminacion (requisito Stack&lt;T&gt;).
    /// </summary>
    public Stack<Inscripcion> PilaEliminadas { get; } = new();

    public List<Inscripcion> ObtenerTodos()
    {
        var lista = new List<Inscripcion>();
        foreach (var r in _dal.ObtenerTodos())
        {
            lista.Add(Mapear(r));
        }
        // foreach recorriendo la coleccion para marcar vencidas visualmente (regla de negocio)
        foreach (var insc in lista)
        {
            if (insc.Estado == "Vigente" && insc.FechaVencimiento < DateTime.Today)
                insc.Estado = "Vencida";
        }
        return lista;
    }

    /// <summary>
    /// Busca si el miembro ya tiene una inscripcion Vigente registrada (para alertar antes de
    /// agregarle un plan nuevo). idExcluirInscripcion se usa al editar, para no compararla consigo misma.
    /// </summary>
    public Inscripcion? ObtenerVigentePorMiembro(int miembroId, int idExcluirInscripcion = 0) =>
        ObtenerTodos().FirstOrDefault(i => i.MiembroId == miembroId && i.Estado == "Vigente" && i.Id != idExcluirInscripcion);

    public List<DetalleInscripcion> ObtenerDetalle(int inscripcionId)
    {
        var lista = new List<DetalleInscripcion>();
        foreach (var d in _dal.ObtenerDetalle(inscripcionId))
        {
            lista.Add(new DetalleInscripcion
            {
                Id = d.Id,
                InscripcionId = d.InscripcionId,
                Concepto = d.Concepto,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            });
        }
        return lista;
    }

    public List<string> Validar(Inscripcion i)
    {
        var errores = new List<string>();
        if (i.MiembroId <= 0) errores.Add("Seleccione un miembro.");
        if (i.PlanId <= 0) errores.Add("Seleccione un plan.");
        if (i.Detalle.Count == 0) errores.Add("Agregue al menos un item al detalle.");
        if (i.Detalle.Any(d => d.Cantidad <= 0)) errores.Add("Las cantidades deben ser mayores a cero.");
        if (i.Detalle.Any(d => d.PrecioUnitario < 0)) errores.Add("Los precios no pueden ser negativos.");
        return errores;
    }

    /// <summary>Registra la inscripcion, calcula vencimiento/total con el plan y la encola como pendiente.</summary>
    public int Registrar(Inscripcion i)
    {
        ExigirPuedeInscribir();
        var plan = _planDal.ObtenerPorId(i.PlanId) ?? throw new InvalidOperationException("Plan no encontrado.");
        i.FechaVencimiento = new Plan { TipoDuracion = plan.TipoDuracion }.CalcularVencimiento(i.FechaInscripcion);
        i.Total = i.Detalle.Sum(d => d.Subtotal);
        i.Estado = "Vigente";

        var cab = new InscripcionRow
        {
            MiembroId = i.MiembroId,
            PlanId = i.PlanId,
            EntrenadorId = i.EntrenadorId,
            FechaInscripcion = i.FechaInscripcion,
            FechaVencimiento = i.FechaVencimiento,
            Estado = i.Estado,
            Total = i.Total
        };
        var detalle = i.Detalle.Select(d => new DetalleInscripcionRow
        {
            Concepto = d.Concepto,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario
        }).ToList();

        i.Id = _dal.InsertarConDetalle(cab, detalle);
        ColaPendientes.Enqueue(i);
        _auditoria.Registrar("INSCRIPCION_CREADA", "Inscripcion", i.Id,
            $"Miembro #{i.MiembroId}, plan #{i.PlanId}, total ${i.Total:N0}");
        return i.Id;
    }

    /// <summary>Actualiza una inscripcion existente (recalculando vencimiento y total).</summary>
    public void Actualizar(Inscripcion i)
    {
        SesionActual.Exigir(Permisos.InscripcionesEditar);
        var plan = _planDal.ObtenerPorId(i.PlanId) ?? throw new InvalidOperationException("Plan no encontrado.");
        i.FechaVencimiento = new Plan { TipoDuracion = plan.TipoDuracion }.CalcularVencimiento(i.FechaInscripcion);
        i.Total = i.Detalle.Sum(d => d.Subtotal);

        var cab = new InscripcionRow
        {
            Id = i.Id,
            MiembroId = i.MiembroId,
            PlanId = i.PlanId,
            EntrenadorId = i.EntrenadorId,
            FechaInscripcion = i.FechaInscripcion,
            FechaVencimiento = i.FechaVencimiento,
            Estado = i.Estado,
            Total = i.Total
        };
        var detalle = i.Detalle.Select(d => new DetalleInscripcionRow
        {
            Concepto = d.Concepto,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario
        }).ToList();

        _dal.ActualizarConDetalle(cab, detalle);
        _auditoria.Registrar("INSCRIPCION_ACTUALIZADA", "Inscripcion", i.Id, $"Total ${i.Total:N0}");
    }

    /// <summary>Atiende (retira) el siguiente turno pendiente de la cola FIFO.</summary>
    public Inscripcion? AtenderSiguientePendiente() =>
        ColaPendientes.Count > 0 ? ColaPendientes.Dequeue() : null;

    public void Eliminar(int id)
    {
        SesionActual.Exigir(Permisos.InscripcionesEditar);

        // Regla contable: una inscripcion con pagos no se borra (se cancela desde Membresias).
        if (_pagoDal.InscripcionTienePagos(id))
            throw new InvalidOperationException(
                "No se puede eliminar una inscripcion que tiene pagos registrados. Cancelela desde el modulo de Membresias.");

        var row = _dal.ObtenerPorId(id);
        if (row != null)
        {
            var entidad = Mapear(row);
            entidad.Detalle = ObtenerDetalle(id);
            PilaEliminadas.Push(entidad); // guarda para poder deshacer
        }
        _dal.Eliminar(id);
        _auditoria.Registrar("INSCRIPCION_ELIMINADA", "Inscripcion", id, null);
    }

    /// <summary>Deshace la ultima eliminacion de la sesion, reinsertando la inscripcion con su detalle.</summary>
    public Inscripcion? Deshacer()
    {
        if (PilaEliminadas.Count == 0) return null;
        var i = PilaEliminadas.Pop();

        var cab = new InscripcionRow
        {
            MiembroId = i.MiembroId,
            PlanId = i.PlanId,
            EntrenadorId = i.EntrenadorId,
            FechaInscripcion = i.FechaInscripcion,
            FechaVencimiento = i.FechaVencimiento,
            Estado = i.Estado,
            Total = i.Total
        };
        var detalle = i.Detalle.Select(d => new DetalleInscripcionRow
        {
            Concepto = d.Concepto,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario
        }).ToList();

        i.Id = _dal.InsertarConDetalle(cab, detalle);
        _auditoria.Registrar("INSCRIPCION_RESTAURADA", "Inscripcion", i.Id, "Deshacer eliminacion");
        return i;
    }

    private static Inscripcion Mapear(InscripcionRow r) => new Inscripcion
    {
        Id = r.Id,
        MiembroId = r.MiembroId,
        MiembroNombre = r.MiembroNombre,
        PlanId = r.PlanId,
        PlanNombre = r.PlanNombre,
        EntrenadorId = r.EntrenadorId,
        EntrenadorNombre = r.EntrenadorNombre,
        FechaInscripcion = r.FechaInscripcion,
        FechaVencimiento = r.FechaVencimiento,
        Estado = r.Estado,
        Total = r.Total
    };
}
