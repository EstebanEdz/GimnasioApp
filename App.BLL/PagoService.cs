using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class PagoService
{
    private readonly PagoDAL _dal = new();
    private readonly AuditoriaService _auditoria = new();

    /// <summary>Inscripciones sobre las que se puede registrar un pago (todas menos las canceladas).</summary>
    public List<Membresia> ObtenerInscripcionesParaPago()
    {
        SesionActual.Exigir(Permisos.PagosVer);
        return new MembresiaDAL().Listar(null, null, null, false)
            .Where(m => m.EstadoReal != EstadosMembresia.Cancelada)
            .OrderByDescending(m => m.InscripcionId)
            .Select(MembresiaService.Mapear)
            .ToList();
    }

    public SaldoInscripcion ObtenerSaldo(int inscripcionId)
    {
        SesionActual.Exigir(Permisos.PagosVer);
        var r = _dal.ObtenerSaldo(inscripcionId)
                ?? throw new InvalidOperationException("La inscripcion no existe.");
        return new SaldoInscripcion
        {
            InscripcionId = r.InscripcionId,
            Total = r.Total,
            Pagado = r.Pagado,
            Saldo = r.Saldo
        };
    }

    public List<Pago> ObtenerPorInscripcion(int inscripcionId)
    {
        SesionActual.Exigir(Permisos.PagosVer);
        return _dal.ObtenerPorInscripcion(inscripcionId).Select(Mapear).ToList();
    }

    public List<Pago> ObtenerPorRango(DateTime desde, DateTime hasta)
    {
        SesionActual.Exigir(Permisos.PagosVer);
        return _dal.ObtenerPorRango(desde, hasta).Select(Mapear).ToList();
    }

    /// <summary>Valida el pago antes de guardarlo. Devuelve la lista de errores (vacia si es valido).</summary>
    public List<string> Validar(int inscripcionId, decimal monto, string? metodo, string? referencia)
    {
        var errores = new List<string>();

        if (inscripcionId <= 0) errores.Add("Seleccione una inscripcion.");
        if (monto <= 0) errores.Add("El monto debe ser mayor a cero.");
        if (monto > 1_000_000_000m) errores.Add("El monto es demasiado alto.");

        if (string.IsNullOrWhiteSpace(metodo) || !MetodosPago.Todos.Contains(metodo))
            errores.Add("Seleccione un metodo de pago valido.");
        else if (MetodosPago.RequiereReferencia(metodo) && string.IsNullOrWhiteSpace(referencia))
            errores.Add($"El pago por {metodo} requiere el numero de comprobante o referencia.");

        if (!string.IsNullOrWhiteSpace(referencia) && referencia.Trim().Length > 50)
            errores.Add("La referencia no puede superar 50 caracteres.");

        if (errores.Count == 0)
        {
            // Mensajes claros para el cajero. La verificacion DEFINITIVA se repite dentro de la
            // transaccion en PagoDAL.Registrar (por si alguien pago mientras esta pantalla estaba abierta).
            var saldo = _dal.ObtenerSaldo(inscripcionId);
            if (saldo == null) errores.Add("La inscripcion no existe.");
            else if (saldo.Saldo <= 0) errores.Add("Esta inscripcion ya esta pagada por completo.");
            else if (monto > saldo.Saldo) errores.Add($"El monto supera el saldo pendiente (${saldo.Saldo:N0}).");
        }

        return errores;
    }

    /// <summary>Registra un pago (o abono). Devuelve el Id del pago.</summary>
    public int Registrar(Pago pago)
    {
        SesionActual.Exigir(Permisos.PagosRegistrar);

        pago.Monto = Math.Round(pago.Monto, 2);
        var errores = Validar(pago.InscripcionId, pago.Monto, pago.MetodoPago, pago.Referencia);
        if (errores.Count > 0) throw new InvalidOperationException(string.Join("\n", errores));

        var row = new PagoRow
        {
            InscripcionId = pago.InscripcionId,
            Monto = pago.Monto,
            MetodoPago = pago.MetodoPago,
            Referencia = pago.Referencia?.Trim() ?? string.Empty,
            Observacion = pago.Observacion?.Trim() ?? string.Empty,
            UsuarioId = SesionActual.UsuarioId
        };

        pago.Id = _dal.Registrar(row);
        _auditoria.Registrar("PAGO_REGISTRADO", "Pago", pago.Id,
            $"Inscripcion #{pago.InscripcionId}, ${pago.Monto:N0} por {pago.MetodoPago}");
        return pago.Id;
    }

    /// <summary>Anula un pago con motivo obligatorio. El pago no se borra: queda en el historial.</summary>
    public void Anular(int pagoId, string? motivo)
    {
        SesionActual.Exigir(Permisos.PagosAnular);

        if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 5)
            throw new InvalidOperationException("Escriba el motivo de la anulacion (minimo 5 caracteres).");
        if (motivo.Trim().Length > 200)
            throw new InvalidOperationException("El motivo no puede superar 200 caracteres.");

        var pago = _dal.ObtenerPorId(pagoId) ?? throw new InvalidOperationException("El pago no existe.");
        if (pago.Estado == "Anulado") throw new InvalidOperationException("Este pago ya estaba anulado.");

        if (!_dal.Anular(pagoId, motivo.Trim()))
            throw new InvalidOperationException("No fue posible anular el pago (puede que ya estuviera anulado).");

        _auditoria.Registrar("PAGO_ANULADO", "Pago", pagoId,
            $"Inscripcion #{pago.InscripcionId}, ${pago.Monto:N0}. Motivo: {motivo.Trim()}");
    }

    /// <summary>Para validar antes de eliminar una inscripcion: si tiene pagos no se puede borrar.</summary>
    public bool InscripcionTienePagos(int inscripcionId) => _dal.InscripcionTienePagos(inscripcionId);

    private static Pago Mapear(PagoRow r) => new Pago
    {
        Id = r.Id,
        InscripcionId = r.InscripcionId,
        Miembro = r.MiembroNombre,
        Plan = r.PlanNombre,
        FechaPago = r.FechaPago,
        Monto = r.Monto,
        MetodoPago = r.MetodoPago,
        Referencia = r.Referencia,
        Estado = r.Estado,
        Usuario = r.UsuarioNombre,
        Observacion = r.Observacion,
        MotivoAnulacion = r.MotivoAnulacion
    };
}
