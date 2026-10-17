using Microsoft.Data.SqlClient;

namespace App.DAL;

public class PerfilRow
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class PerfilDAL
{
    public List<PerfilRow> ObtenerTodos()
    {
        var lista = new List<PerfilRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("SELECT Id, Nombre FROM dbo.Perfiles ORDER BY Id", cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new PerfilRow
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                });
            }
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
        return lista;
    }
}
