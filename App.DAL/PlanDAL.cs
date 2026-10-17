using Microsoft.Data.SqlClient;

namespace App.DAL;

public class PlanRow
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoDuracion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool IncluyeClases { get; set; }
    public bool Activo { get; set; }
}

public class PlanDAL
{
    private const string Select = "SELECT Id, Nombre, TipoDuracion, Precio, IncluyeClases, Activo FROM dbo.Planes";

    private static PlanRow Mapear(SqlDataReader r) => new PlanRow
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        TipoDuracion = r.GetString(2),
        Precio = r.GetDecimal(3),
        IncluyeClases = r.GetBoolean(4),
        Activo = r.GetBoolean(5)
    };

    public List<PlanRow> ObtenerTodos(bool soloActivos = false)
    {
        var lista = new List<PlanRow>();
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

    public PlanRow? ObtenerPorId(int id)
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

    public bool ExisteNombre(string nombre, int idExcluir = 0)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Planes WHERE Nombre=@n AND Id<>@id", cn);
            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.AddWithValue("@id", idExcluir);
            return (int)cmd.ExecuteScalar() > 0;
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public int Insertar(PlanRow p)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Planes (Nombre, TipoDuracion, Precio, IncluyeClases, Activo)
                OUTPUT INSERTED.Id
                VALUES (@n, @t, @p, @c, @a)", cn);
            cmd.Parameters.AddWithValue("@n", p.Nombre);
            cmd.Parameters.AddWithValue("@t", p.TipoDuracion);
            cmd.Parameters.AddWithValue("@p", p.Precio);
            cmd.Parameters.AddWithValue("@c", p.IncluyeClases);
            cmd.Parameters.AddWithValue("@a", p.Activo);
            return (int)cmd.ExecuteScalar();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public void Actualizar(PlanRow p)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Planes SET Nombre=@n, TipoDuracion=@t, Precio=@p, IncluyeClases=@c, Activo=@a
                WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@n", p.Nombre);
            cmd.Parameters.AddWithValue("@t", p.TipoDuracion);
            cmd.Parameters.AddWithValue("@p", p.Precio);
            cmd.Parameters.AddWithValue("@c", p.IncluyeClases);
            cmd.Parameters.AddWithValue("@a", p.Activo);
            cmd.Parameters.AddWithValue("@id", p.Id);
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
            using var cmd = new SqlCommand("DELETE FROM dbo.Planes WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }
}
