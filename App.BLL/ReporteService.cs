using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class IngresoMetodo
{
    public string Metodo { get; set; } = string.Empty;
    public int NumPagos { get; set; }
    public decimal Total { get; set; }
    public decimal Porcentaje { get; set; }
}

public class ResumenFinanciero
{
    public decimal Cobrado { get; set; }
    public decimal Facturado { get; set; }
    public decimal CarteraPendiente { get; set; }
    public int MiembrosConSaldo { get; set; }

    /// <summary>Que porcentaje de lo facturado en el rango ya se cobro (0 si no se facturo nada).</summary>
    public decimal PorcentajeRecaudo => Facturado <= 0 ? 0 : Math.Round(Cobrado / Facturado * 100m, 1);
}

public class ReporteService
{
    private readonly ReporteDAL _dal = new();

    public List<MiembrosPorPlanRow> MiembrosPorPlan() => _dal.MiembrosPorPlan();

    public List<InscripcionesPorMesRow> InscripcionesPorMes() => _dal.InscripcionesPorMes();

    public (List<IngresoDetalleRow> detalle, decimal total) IngresosPorRango(DateTime desde, DateTime hasta)
    {
        var detalle = _dal.IngresosPorRango(desde, hasta);
        decimal total = detalle.Sum(x => x.Total);
        return (detalle, total);
    }

    // ---------------- v9: reportes financieros (base caja: pagos realmente cobrados) ----------------

    public ResumenFinanciero ObtenerResumenFinanciero(DateTime desde, DateTime hasta)
    {
        SesionActual.Exigir(Permisos.ReportesFinancieros);
        var r = _dal.ResumenFinanciero(desde, hasta);
        return new ResumenFinanciero
        {
            Cobrado = r.Cobrado,
            Facturado = r.Facturado,
            CarteraPendiente = r.CarteraPendiente,
            MiembrosConSaldo = r.MiembrosConSaldo
        };
    }

    public List<IngresoMesRow> IngresosCobradosPorMes(DateTime desde, DateTime hasta)
    {
        SesionActual.Exigir(Permisos.ReportesFinancieros);
        return _dal.IngresosCobradosPorMes(desde, hasta);
    }

    public List<IngresoMetodo> IngresosCobradosPorMetodo(DateTime desde, DateTime hasta)
    {
        SesionActual.Exigir(Permisos.ReportesFinancieros);
        var filas = _dal.IngresosCobradosPorMetodo(desde, hasta);
        decimal total = filas.Sum(f => f.Total);
        return filas.Select(f => new IngresoMetodo
        {
            Metodo = f.Metodo,
            NumPagos = f.NumPagos,
            Total = f.Total,
            Porcentaje = total <= 0 ? 0 : Math.Round(f.Total / total * 100m, 1)
        }).ToList();
    }

    /// <summary>Membresias vigentes que vencen dentro de N dias (para llamar al miembro y ofrecerle renovar).</summary>
    public List<Membresia> ProximasAVencer(int dias)
    {
        SesionActual.Exigir(Permisos.ReportesFinancieros);
        return new MembresiaDAL().Listar(null, null, dias, false).Select(MembresiaService.Mapear).ToList();
    }

    /// <summary>Cartera: membresias (no canceladas) con saldo pendiente, de mayor a menor deuda.</summary>
    public List<Membresia> Morosos()
    {
        SesionActual.Exigir(Permisos.ReportesFinancieros);
        return new MembresiaDAL().Listar(null, null, null, true)
            .Select(MembresiaService.Mapear)
            .OrderByDescending(m => m.Saldo)
            .ToList();
    }

    public List<EstadoCantidadRow> MembresiasPorEstado()
    {
        SesionActual.Exigir(Permisos.ReportesFinancieros);
        return new MembresiaDAL().ContarPorEstado();
    }
}
