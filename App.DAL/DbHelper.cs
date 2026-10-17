using Microsoft.Data.SqlClient;

namespace App.DAL;

/// <summary>
/// Ayudante para no repetir el mismo bloque "abrir conexion / crear comando / leer / cerrar"
/// en cada metodo. Los DAL nuevos (v9) lo usan; los DAL anteriores siguen con el estilo
/// original y funcionan igual. Siempre usa parametros (@p) para evitar SQL Injection.
/// </summary>
internal static class DbHelper
{
    /// <summary>Ejecuta un SELECT y convierte cada fila con la funcion indicada.</summary>
    public static List<T> Consultar<T>(string sql, Func<SqlDataReader, T> mapear, Action<SqlCommand>? parametros = null)
    {
        var lista = new List<T>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(sql, cn);
            parametros?.Invoke(cmd);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(mapear(reader));
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
        return lista;
    }

    /// <summary>Ejecuta INSERT/UPDATE/DELETE y devuelve las filas afectadas.</summary>
    public static int Ejecutar(string sql, Action<SqlCommand>? parametros = null)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(sql, cn);
            parametros?.Invoke(cmd);
            return cmd.ExecuteNonQuery();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    /// <summary>Ejecuta una consulta que devuelve un solo valor (COUNT, SUM, OUTPUT INSERTED.Id...).</summary>
    public static object? Escalar(string sql, Action<SqlCommand>? parametros = null)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(sql, cn);
            parametros?.Invoke(cmd);
            return cmd.ExecuteScalar();
        }
        finally { if (cn.State == System.Data.ConnectionState.Open) cn.Close(); }
    }

    public static object ONulo(object? valor) => valor ?? DBNull.Value;
}
