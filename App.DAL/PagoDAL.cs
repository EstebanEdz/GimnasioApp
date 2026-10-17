using Microsoft.Data.SqlClient;

namespace App.DAL;

public class PagoRow
{
    public int Id { get; set; }
    public int InscripcionId { get; set; }
    public string MiembroNombre { get; set; } = string.Empty;
    public string PlanNombre { get; set; } = string.Empty;
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public string Referencia { get; set; } = string.Empty;
    public string Estado { get; set; } = "Aplicado";
    public int? UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public string MotivoAnulacion { get; set; } = string.Empty;
}

public class SaldoRow
{
    public int InscripcionId { get; set; }
    public decimal Total { get; set; }
    public decimal Pagado { get; set; }
    public decimal Saldo { get; set; }
}

public class PagoDAL
{
    private const string Select = @"
        SELECT pg.Id, pg.InscripcionId, m.Nombre, p.Nombre, pg.FechaPago, pg.Monto, pg.MetodoPago,
               ISNULL(pg.Referencia, ''), pg.Estado, pg.UsuarioId, ISNULL(u.NombreUsuario, '(eliminado)'),
               ISNULL(pg.Observacion, ''), ISNULL(pg.MotivoAnulacion, '')
        FROM dbo.Pagos pg
        INNER JOIN dbo.Inscripciones i ON i.Id = pg.InscripcionId
        INNER JOIN dbo.Miembros m ON m.Id = i.MiembroId
        INNER JOIN dbo.Planes p ON p.Id = i.PlanId
        LEFT JOIN dbo.Usuarios u ON u.Id = pg.UsuarioId";

    private static PagoRow Mapear(SqlDataReader r) => new PagoRow
    {
        Id = r.GetInt32(0),
        InscripcionId = r.GetInt32(1),
        MiembroNombre = r.GetString(2),
        PlanNombre = r.GetString(3),
        FechaPago = r.GetDateTime(4),
        Monto = r.GetDecimal(5),
        MetodoPago = r.GetString(6),
        Referencia = r.GetString(7),
        Estado = r.GetString(8),
        UsuarioId = r.IsDBNull(9) ? null : r.GetInt32(9),
        UsuarioNombre = r.GetString(10),
        Observacion = r.GetString(11),
        MotivoAnulacion = r.GetString(12)
    };

    public List<PagoRow> ObtenerPorInscripcion(int inscripcionId) => DbHelper.Consultar(
        Select + " WHERE pg.InscripcionId = @id ORDER BY pg.FechaPago DESC, pg.Id DESC",
        Mapear,
        cmd => cmd.Parameters.AddWithValue("@id", inscripcionId));

    public List<PagoRow> ObtenerPorRango(DateTime desde, DateTime hasta) => DbHelper.Consultar(
        Select + " WHERE pg.FechaPago >= @d AND pg.FechaPago < @h ORDER BY pg.FechaPago DESC, pg.Id DESC",
        Mapear,
        cmd =>
        {
            cmd.Parameters.AddWithValue("@d", desde.Date);
            cmd.Parameters.AddWithValue("@h", hasta.Date.AddDays(1));
        });

    public PagoRow? ObtenerPorId(int id) =>
        DbHelper.Consultar(Select + " WHERE pg.Id = @id", Mapear,
            cmd => cmd.Parameters.AddWithValue("@id", id)).FirstOrDefault();

    public SaldoRow? ObtenerSaldo(int inscripcionId) => DbHelper.Consultar(
        "SELECT InscripcionId, Total, Pagado, Saldo FROM dbo.vw_SaldoInscripcion WHERE InscripcionId = @id",
        r => new SaldoRow
        {
            InscripcionId = r.GetInt32(0),
            Total = r.GetDecimal(1),
            Pagado = r.GetDecimal(2),
            Saldo = r.GetDecimal(3)
        },
        cmd => cmd.Parameters.AddWithValue("@id", inscripcionId)).FirstOrDefault();

    /// <summary>
    /// Registra un pago de forma ATOMICA. Dentro de una transaccion:
    ///   1) bloquea la fila de la inscripcion (UPDLOCK, HOLDLOCK) para que dos cajeros
    ///      no puedan pagar el mismo saldo al mismo tiempo,
    ///   2) recalcula lo ya pagado,
    ///   3) rechaza el pago si la inscripcion esta cancelada o si el monto supera el saldo.
    /// </summary>
    public int Registrar(PagoRow p)
    {
        using var cn = Conexion.ObtenerConexion();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            decimal total;
            string estadoInscripcion;
            using (var cmd = new SqlCommand(
                "SELECT Total, Estado FROM dbo.Inscripciones WITH (UPDLOCK, HOLDLOCK) WHERE Id = @id", cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", p.InscripcionId);
                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) throw new InvalidOperationException("La inscripcion no existe.");
                total = reader.GetDecimal(0);
                estadoInscripcion = reader.GetString(1);
            }

            if (estadoInscripcion == "Cancelada")
                throw new InvalidOperationException("No se pueden registrar pagos en una membresia cancelada.");

            decimal pagado;
            using (var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(Monto), 0) FROM dbo.Pagos WHERE InscripcionId = @id AND Estado = 'Aplicado'", cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", p.InscripcionId);
                pagado = (decimal)cmd.ExecuteScalar()!;
            }

            decimal saldo = total - pagado;
            if (saldo <= 0)
                throw new InvalidOperationException("Esta inscripcion ya esta pagada por completo.");
            if (p.Monto > saldo)
                throw new InvalidOperationException($"El monto (${p.Monto:N0}) supera el saldo pendiente (${saldo:N0}).");

            int nuevoId;
            using (var cmd = new SqlCommand(@"
                INSERT INTO dbo.Pagos (InscripcionId, Monto, MetodoPago, Referencia, UsuarioId, Observacion)
                OUTPUT INSERTED.Id
                VALUES (@i, @monto, @metodo, @ref, @uid, @obs)", cn, tx))
            {
                cmd.Parameters.AddWithValue("@i", p.InscripcionId);
                cmd.Parameters.AddWithValue("@monto", p.Monto);
                cmd.Parameters.AddWithValue("@metodo", p.MetodoPago);
                cmd.Parameters.AddWithValue("@ref", string.IsNullOrWhiteSpace(p.Referencia) ? DBNull.Value : p.Referencia);
                cmd.Parameters.AddWithValue("@uid", DbHelper.ONulo(p.UsuarioId));
                cmd.Parameters.AddWithValue("@obs", string.IsNullOrWhiteSpace(p.Observacion) ? DBNull.Value : p.Observacion);
                nuevoId = (int)cmd.ExecuteScalar()!;
            }

            tx.Commit();
            return nuevoId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>Anula un pago (no se borra: queda el historial). Devuelve false si ya estaba anulado o no existe.</summary>
    public bool Anular(int pagoId, string motivo)
    {
        int filas = DbHelper.Ejecutar(@"
            UPDATE dbo.Pagos
            SET Estado = 'Anulado', FechaAnulacion = SYSDATETIME(), MotivoAnulacion = @motivo
            WHERE Id = @id AND Estado = 'Aplicado'",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@motivo", motivo);
                cmd.Parameters.AddWithValue("@id", pagoId);
            });
        return filas > 0;
    }

    public bool InscripcionTienePagos(int inscripcionId)
    {
        object? n = DbHelper.Escalar("SELECT COUNT(*) FROM dbo.Pagos WHERE InscripcionId = @id",
            cmd => cmd.Parameters.AddWithValue("@id", inscripcionId));
        return n != null && (int)n > 0;
    }
}
