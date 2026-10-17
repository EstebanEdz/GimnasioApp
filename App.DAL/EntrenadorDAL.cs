using Microsoft.Data.SqlClient;

namespace App.DAL;

public class EntrenadorRow
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }
    public bool Activo { get; set; }
}

public class EntrenadorDAL
{
    private const string Select = "SELECT Id, Nombre, Documento, Telefono, Especialidad, TarifaHora, Activo FROM dbo.Entrenadores";

    private static EntrenadorRow Mapear(SqlDataReader r) => new EntrenadorRow
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Documento = r.GetString(2),
        Telefono = r.IsDBNull(3) ? string.Empty : r.GetString(3),
        Especialidad = r.GetString(4),
        TarifaHora = r.GetDecimal(5),
        Activo = r.GetBoolean(6)
    };

    public List<EntrenadorRow> ObtenerTodos(bool soloActivos = false)
    {
        var lista = new List<EntrenadorRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            string sql = Select + (soloActivos ? " WHERE Activo = 1" : "") + " ORDER BY Id";
            using var cmd = new SqlCommand(sql, cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    public bool ExisteDocumento(string documento, int idExcluir = 0)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Entrenadores WHERE Documento=@d AND Id<>@id", cn);
            cmd.Parameters.AddWithValue("@d", documento);
            cmd.Parameters.AddWithValue("@id", idExcluir);
            return (int)cmd.ExecuteScalar() > 0;
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public int Insertar(EntrenadorRow e)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Entrenadores (Nombre, Documento, Telefono, Especialidad, TarifaHora, Activo)
                OUTPUT INSERTED.Id
                VALUES (@n, @d, @t, @es, @ta, @a)", cn);
            cmd.Parameters.AddWithValue("@n", e.Nombre);
            cmd.Parameters.AddWithValue("@d", e.Documento);
            cmd.Parameters.AddWithValue("@t", (object?)e.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@es", e.Especialidad);
            cmd.Parameters.AddWithValue("@ta", e.TarifaHora);
            cmd.Parameters.AddWithValue("@a", e.Activo);
            return (int)cmd.ExecuteScalar();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public void Actualizar(EntrenadorRow e)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Entrenadores SET Nombre=@n, Documento=@d, Telefono=@t,
                    Especialidad=@es, TarifaHora=@ta, Activo=@a WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@n", e.Nombre);
            cmd.Parameters.AddWithValue("@d", e.Documento);
            cmd.Parameters.AddWithValue("@t", (object?)e.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@es", e.Especialidad);
            cmd.Parameters.AddWithValue("@ta", e.TarifaHora);
            cmd.Parameters.AddWithValue("@a", e.Activo);
            cmd.Parameters.AddWithValue("@id", e.Id);
            cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public void Eliminar(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("DELETE FROM dbo.Entrenadores WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }
}
