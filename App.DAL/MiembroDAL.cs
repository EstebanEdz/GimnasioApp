using Microsoft.Data.SqlClient;

namespace App.DAL;

public class MiembroRow
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public bool Activo { get; set; }
}

public class MiembroDAL
{
    private static MiembroRow Mapear(SqlDataReader r) => new MiembroRow
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Documento = r.GetString(2),
        Telefono = r.IsDBNull(3) ? string.Empty : r.GetString(3),
        Correo = r.IsDBNull(4) ? string.Empty : r.GetString(4),
        FechaIngreso = r.GetDateTime(5),
        Activo = r.GetBoolean(6)
    };

    private const string Select = "SELECT Id, Nombre, Documento, Telefono, Correo, FechaIngreso, Activo FROM dbo.Miembros";

    public List<MiembroRow> ObtenerTodos()
    {
        var lista = new List<MiembroRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(Select + " ORDER BY Id", cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    public MiembroRow? ObtenerPorId(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(Select + " WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public List<MiembroRow> Buscar(string texto)
    {
        var lista = new List<MiembroRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(Select + " WHERE Nombre LIKE @t OR Documento LIKE @t ORDER BY Id", cn);
            cmd.Parameters.AddWithValue("@t", $"%{texto}%");
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
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Miembros WHERE Documento=@d AND Id<>@id", cn);
            cmd.Parameters.AddWithValue("@d", documento);
            cmd.Parameters.AddWithValue("@id", idExcluir);
            return (int)cmd.ExecuteScalar() > 0;
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    /// <summary>Compara por nombre completo (recorta espacios; la colacion por defecto de SQL Server ya es insensible a mayusculas).</summary>
    public bool ExisteNombre(string nombre, int idExcluir = 0)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Miembros WHERE LTRIM(RTRIM(Nombre))=LTRIM(RTRIM(@n)) AND Id<>@id", cn);
            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.AddWithValue("@id", idExcluir);
            return (int)cmd.ExecuteScalar() > 0;
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public int Insertar(MiembroRow m)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Miembros (Nombre, Documento, Telefono, Correo, FechaIngreso, Activo)
                OUTPUT INSERTED.Id
                VALUES (@n, @d, @t, @c, @f, @a)", cn);
            cmd.Parameters.AddWithValue("@n", m.Nombre);
            cmd.Parameters.AddWithValue("@d", m.Documento);
            cmd.Parameters.AddWithValue("@t", (object?)m.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@c", (object?)m.Correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@f", m.FechaIngreso);
            cmd.Parameters.AddWithValue("@a", m.Activo);
            return (int)cmd.ExecuteScalar();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public void Actualizar(MiembroRow m)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Miembros SET Nombre=@n, Documento=@d, Telefono=@t, Correo=@c,
                    FechaIngreso=@f, Activo=@a WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@n", m.Nombre);
            cmd.Parameters.AddWithValue("@d", m.Documento);
            cmd.Parameters.AddWithValue("@t", (object?)m.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@c", (object?)m.Correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@f", m.FechaIngreso);
            cmd.Parameters.AddWithValue("@a", m.Activo);
            cmd.Parameters.AddWithValue("@id", m.Id);
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
            using var cmd = new SqlCommand("DELETE FROM dbo.Miembros WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }
}
