using Microsoft.Data.SqlClient;

namespace App.DAL;

public class InscripcionRow
{
    public int Id { get; set; }
    public int MiembroId { get; set; }
    public string MiembroNombre { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string PlanNombre { get; set; } = string.Empty;
    public int? EntrenadorId { get; set; }
    public string EntrenadorNombre { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Estado { get; set; } = "Vigente";
    public decimal Total { get; set; }
}

public class DetalleInscripcionRow
{
    public int Id { get; set; }
    public int InscripcionId { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class InscripcionDAL
{
    private const string Select = @"
        SELECT i.Id, i.MiembroId, m.Nombre, i.PlanId, p.Nombre, i.EntrenadorId,
               ISNULL(e.Nombre, '(Sin entrenador)'), i.FechaInscripcion, i.FechaVencimiento, i.Estado, i.Total
        FROM dbo.Inscripciones i
        INNER JOIN dbo.Miembros m ON m.Id = i.MiembroId
        INNER JOIN dbo.Planes p ON p.Id = i.PlanId
        LEFT JOIN dbo.Entrenadores e ON e.Id = i.EntrenadorId";

    private static InscripcionRow Mapear(SqlDataReader r) => new InscripcionRow
    {
        Id = r.GetInt32(0),
        MiembroId = r.GetInt32(1),
        MiembroNombre = r.GetString(2),
        PlanId = r.GetInt32(3),
        PlanNombre = r.GetString(4),
        EntrenadorId = r.IsDBNull(5) ? null : r.GetInt32(5),
        EntrenadorNombre = r.GetString(6),
        FechaInscripcion = r.GetDateTime(7),
        FechaVencimiento = r.GetDateTime(8),
        Estado = r.GetString(9),
        Total = r.GetDecimal(10)
    };

    public List<InscripcionRow> ObtenerTodos()
    {
        var lista = new List<InscripcionRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(Select + " ORDER BY i.Id DESC", cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    public List<DetalleInscripcionRow> ObtenerDetalle(int inscripcionId)
    {
        var lista = new List<DetalleInscripcionRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                SELECT Id, InscripcionId, Concepto, Cantidad, PrecioUnitario, Subtotal
                FROM dbo.DetalleInscripcion WHERE InscripcionId=@id", cn);
            cmd.Parameters.AddWithValue("@id", inscripcionId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DetalleInscripcionRow
                {
                    Id = reader.GetInt32(0),
                    InscripcionId = reader.GetInt32(1),
                    Concepto = reader.GetString(2),
                    Cantidad = reader.GetInt32(3),
                    PrecioUnitario = reader.GetDecimal(4),
                    Subtotal = reader.GetDecimal(5)
                });
            }
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    /// <summary>Inserta la cabecera y su detalle dentro de una sola transaccion.</summary>
    public int InsertarConDetalle(InscripcionRow cab, List<DetalleInscripcionRow> detalle)
    {
        using var cn = Conexion.ObtenerConexion();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Inscripciones (MiembroId, PlanId, EntrenadorId, FechaInscripcion, FechaVencimiento, Estado, Total)
                OUTPUT INSERTED.Id
                VALUES (@m, @p, @e, @fi, @fv, @es, @t)", cn, tx);
            cmd.Parameters.AddWithValue("@m", cab.MiembroId);
            cmd.Parameters.AddWithValue("@p", cab.PlanId);
            cmd.Parameters.AddWithValue("@e", (object?)cab.EntrenadorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fi", cab.FechaInscripcion);
            cmd.Parameters.AddWithValue("@fv", cab.FechaVencimiento);
            cmd.Parameters.AddWithValue("@es", cab.Estado);
            cmd.Parameters.AddWithValue("@t", cab.Total);
            int nuevoId = (int)cmd.ExecuteScalar();

            foreach (var d in detalle)
            {
                using var cmdD = new SqlCommand(@"
                    INSERT INTO dbo.DetalleInscripcion (InscripcionId, Concepto, Cantidad, PrecioUnitario)
                    VALUES (@insc, @c, @cant, @pu)", cn, tx);
                cmdD.Parameters.AddWithValue("@insc", nuevoId);
                cmdD.Parameters.AddWithValue("@c", d.Concepto);
                cmdD.Parameters.AddWithValue("@cant", d.Cantidad);
                cmdD.Parameters.AddWithValue("@pu", d.PrecioUnitario);
                cmdD.ExecuteNonQuery();
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

    /// <summary>Actualiza la cabecera y reemplaza el detalle completo dentro de una transaccion.</summary>
    public void ActualizarConDetalle(InscripcionRow cab, List<DetalleInscripcionRow> detalle)
    {
        using var cn = Conexion.ObtenerConexion();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Inscripciones SET MiembroId=@m, PlanId=@p, EntrenadorId=@e,
                    FechaInscripcion=@fi, FechaVencimiento=@fv, Estado=@es, Total=@t
                WHERE Id=@id", cn, tx);
            cmd.Parameters.AddWithValue("@m", cab.MiembroId);
            cmd.Parameters.AddWithValue("@p", cab.PlanId);
            cmd.Parameters.AddWithValue("@e", (object?)cab.EntrenadorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fi", cab.FechaInscripcion);
            cmd.Parameters.AddWithValue("@fv", cab.FechaVencimiento);
            cmd.Parameters.AddWithValue("@es", cab.Estado);
            cmd.Parameters.AddWithValue("@t", cab.Total);
            cmd.Parameters.AddWithValue("@id", cab.Id);
            cmd.ExecuteNonQuery();

            using var cmdDel = new SqlCommand("DELETE FROM dbo.DetalleInscripcion WHERE InscripcionId=@id", cn, tx);
            cmdDel.Parameters.AddWithValue("@id", cab.Id);
            cmdDel.ExecuteNonQuery();

            foreach (var d in detalle)
            {
                using var cmdD = new SqlCommand(@"
                    INSERT INTO dbo.DetalleInscripcion (InscripcionId, Concepto, Cantidad, PrecioUnitario)
                    VALUES (@insc, @c, @cant, @pu)", cn, tx);
                cmdD.Parameters.AddWithValue("@insc", cab.Id);
                cmdD.Parameters.AddWithValue("@c", d.Concepto);
                cmdD.Parameters.AddWithValue("@cant", d.Cantidad);
                cmdD.Parameters.AddWithValue("@pu", d.PrecioUnitario);
                cmdD.ExecuteNonQuery();
            }

            tx.Commit();
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

    public void ActualizarEstado(int id, string estado)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("UPDATE dbo.Inscripciones SET Estado=@e WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@e", estado);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    /// <summary>Devuelve la fila completa (para poder apilarla y permitir "deshacer") y la elimina.</summary>
    public InscripcionRow? ObtenerPorId(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(Select + " WHERE i.Id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public void Eliminar(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("DELETE FROM dbo.Inscripciones WHERE Id=@id", cn); // DetalleInscripcion cae en cascada
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }
}
