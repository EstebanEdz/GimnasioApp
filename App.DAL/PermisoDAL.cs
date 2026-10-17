using Microsoft.Data.SqlClient;

namespace App.DAL;

public class PermisoRow
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
}

public class PermisoDAL
{
    public List<PermisoRow> ObtenerTodos() => DbHelper.Consultar(
        "SELECT Id, Codigo, Descripcion, Modulo FROM dbo.Permisos ORDER BY Modulo, Id",
        r => new PermisoRow
        {
            Id = r.GetInt32(0),
            Codigo = r.GetString(1),
            Descripcion = r.GetString(2),
            Modulo = r.GetString(3)
        });

    /// <summary>Codigos de permiso que tiene un perfil (JOIN entre PerfilPermisos y Permisos).</summary>
    public List<string> ObtenerCodigosPorPerfil(int perfilId) => DbHelper.Consultar(@"
        SELECT p.Codigo
        FROM dbo.PerfilPermisos pp
        INNER JOIN dbo.Permisos p ON p.Id = pp.PermisoId
        WHERE pp.PerfilId = @perfil",
        r => r.GetString(0),
        cmd => cmd.Parameters.AddWithValue("@perfil", perfilId));

    /// <summary>Reemplaza TODOS los permisos de un perfil dentro de una sola transaccion (o se guardan todos o ninguno).</summary>
    public void GuardarPermisosDePerfil(int perfilId, IEnumerable<string> codigos)
    {
        using var cn = Conexion.ObtenerConexion();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            using (var del = new SqlCommand("DELETE FROM dbo.PerfilPermisos WHERE PerfilId = @perfil", cn, tx))
            {
                del.Parameters.AddWithValue("@perfil", perfilId);
                del.ExecuteNonQuery();
            }

            foreach (string codigo in codigos.Distinct())
            {
                using var ins = new SqlCommand(@"
                    INSERT INTO dbo.PerfilPermisos (PerfilId, PermisoId)
                    SELECT @perfil, Id FROM dbo.Permisos WHERE Codigo = @codigo", cn, tx);
                ins.Parameters.AddWithValue("@perfil", perfilId);
                ins.Parameters.AddWithValue("@codigo", codigo);
                ins.ExecuteNonQuery();
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
}
