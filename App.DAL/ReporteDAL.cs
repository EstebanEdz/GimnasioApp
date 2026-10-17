using Microsoft.Data.SqlClient;

namespace App.DAL;

public class MiembrosPorPlanRow
{
    public string Plan { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class InscripcionesPorMesRow
{
    public string Mes { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class IngresoDetalleRow
{
    public DateTime Fecha { get; set; }
    public string Miembro { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class IngresoMesRow
{
    public string Mes { get; set; } = string.Empty;
    public int NumPagos { get; set; }
    public decimal Total { get; set; }
}

public class IngresoMetodoRow
{
    public string Metodo { get; set; } = string.Empty;
    public int NumPagos { get; set; }
    public decimal Total { get; set; }
}

public class ResumenFinancieroRow
{
    public decimal Cobrado { get; set; }          // pagos aplicados dentro del rango
    public decimal Facturado { get; set; }        // total de inscripciones creadas en el rango
    public decimal CarteraPendiente { get; set; } // saldo que hoy deben los miembros (membresias no canceladas)
    public int MiembrosConSaldo { get; set; }
}

public class ReporteDAL
{
    /// <summary>Reporte SENCILLO: Miembros (inscripciones vigentes) agrupados por plan.</summary>
    public List<MiembrosPorPlanRow> MiembrosPorPlan()
    {
        var lista = new List<MiembrosPorPlanRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                SELECT p.Nombre, COUNT(*) AS Cantidad
                FROM dbo.Inscripciones i
                INNER JOIN dbo.Planes p ON p.Id = i.PlanId
                GROUP BY p.Nombre
                ORDER BY Cantidad DESC", cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new MiembrosPorPlanRow { Plan = reader.GetString(0), Cantidad = reader.GetInt32(1) });
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    /// <summary>Reporte CON GRAFICO: inscripciones agrupadas por mes.</summary>
    public List<InscripcionesPorMesRow> InscripcionesPorMes()
    {
        var lista = new List<InscripcionesPorMesRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                SELECT FORMAT(FechaInscripcion, 'yyyy-MM') AS Mes, COUNT(*) AS Cantidad
                FROM dbo.Inscripciones
                GROUP BY FORMAT(FechaInscripcion, 'yyyy-MM')
                ORDER BY Mes", cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new InscripcionesPorMesRow { Mes = reader.GetString(0), Cantidad = reader.GetInt32(1) });
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    /// <summary>Reporte CALCULADO: ingresos totales y detalle en un rango de fechas.</summary>
    public List<IngresoDetalleRow> IngresosPorRango(DateTime desde, DateTime hasta)
    {
        var lista = new List<IngresoDetalleRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                SELECT i.FechaInscripcion, m.Nombre, p.Nombre, i.Total
                FROM dbo.Inscripciones i
                INNER JOIN dbo.Miembros m ON m.Id = i.MiembroId
                INNER JOIN dbo.Planes p ON p.Id = i.PlanId
                WHERE i.FechaInscripcion BETWEEN @d AND @h
                ORDER BY i.FechaInscripcion", cn);
            cmd.Parameters.AddWithValue("@d", desde.Date);
            cmd.Parameters.AddWithValue("@h", hasta.Date);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new IngresoDetalleRow
                {
                    Fecha = reader.GetDateTime(0),
                    Miembro = reader.GetString(1),
                    Plan = reader.GetString(2),
                    Total = reader.GetDecimal(3)
                });
            }
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    // ====================== v9: REPORTES FINANCIEROS (base caja = pagos reales) ======================

    /// <summary>Ingresos cobrados por mes (solo pagos Aplicados) dentro del rango.</summary>
    public List<IngresoMesRow> IngresosCobradosPorMes(DateTime desde, DateTime hasta) => DbHelper.Consultar(@"
        SELECT FORMAT(FechaPago, 'yyyy-MM') AS Mes, COUNT(*) AS NumPagos, SUM(Monto) AS Total
        FROM dbo.Pagos
        WHERE Estado = 'Aplicado' AND FechaPago >= @d AND FechaPago < @h
        GROUP BY FORMAT(FechaPago, 'yyyy-MM')
        ORDER BY Mes",
        r => new IngresoMesRow { Mes = r.GetString(0), NumPagos = r.GetInt32(1), Total = r.GetDecimal(2) },
        cmd =>
        {
            cmd.Parameters.AddWithValue("@d", desde.Date);
            cmd.Parameters.AddWithValue("@h", hasta.Date.AddDays(1));
        });

    /// <summary>Ingresos cobrados agrupados por metodo de pago.</summary>
    public List<IngresoMetodoRow> IngresosCobradosPorMetodo(DateTime desde, DateTime hasta) => DbHelper.Consultar(@"
        SELECT MetodoPago, COUNT(*) AS NumPagos, SUM(Monto) AS Total
        FROM dbo.Pagos
        WHERE Estado = 'Aplicado' AND FechaPago >= @d AND FechaPago < @h
        GROUP BY MetodoPago
        ORDER BY Total DESC",
        r => new IngresoMetodoRow { Metodo = r.GetString(0), NumPagos = r.GetInt32(1), Total = r.GetDecimal(2) },
        cmd =>
        {
            cmd.Parameters.AddWithValue("@d", desde.Date);
            cmd.Parameters.AddWithValue("@h", hasta.Date.AddDays(1));
        });

    /// <summary>KPIs: cobrado vs facturado en el rango, y cartera pendiente actual.</summary>
    public ResumenFinancieroRow ResumenFinanciero(DateTime desde, DateTime hasta)
    {
        var resumen = new ResumenFinancieroRow();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                SELECT
                    (SELECT ISNULL(SUM(Monto), 0) FROM dbo.Pagos
                      WHERE Estado = 'Aplicado' AND FechaPago >= @d AND FechaPago < @h),
                    (SELECT ISNULL(SUM(Total), 0) FROM dbo.Inscripciones
                      WHERE Estado <> 'Cancelada' AND FechaInscripcion >= @d AND FechaInscripcion < @h),
                    (SELECT ISNULL(SUM(Saldo), 0) FROM dbo.vw_Membresia
                      WHERE EstadoReal <> 'Cancelada' AND Saldo > 0),
                    (SELECT COUNT(DISTINCT MiembroId) FROM dbo.vw_Membresia
                      WHERE EstadoReal <> 'Cancelada' AND Saldo > 0)", cn);
            cmd.Parameters.AddWithValue("@d", desde.Date);
            cmd.Parameters.AddWithValue("@h", hasta.Date.AddDays(1));
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                resumen.Cobrado = reader.GetDecimal(0);
                resumen.Facturado = reader.GetDecimal(1);
                resumen.CarteraPendiente = reader.GetDecimal(2);
                resumen.MiembrosConSaldo = reader.GetInt32(3);
            }
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return resumen;
    }
}
