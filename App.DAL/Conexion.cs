using System.Configuration;
using Microsoft.Data.SqlClient;

namespace App.DAL;

/// <summary>
/// Punto unico de acceso a la cadena de conexion y creacion de SqlConnection.
/// Ningun otro tipo fuera de App.DAL debe instanciar SqlConnection / SqlCommand directamente.
///
/// Orden de busqueda de la cadena de conexion:
///   1) Variable de entorno GYM_DB_CONNECTION  (ideal para CI o para credenciales que NO deben ir en el repo)
///   2) &lt;connectionStrings&gt;/GymDbConnection en App.config
/// </summary>
public static class Conexion
{
    public const string VariableEntorno = "GYM_DB_CONNECTION";

    public static string CadenaConexion
    {
        get
        {
            string? deEntorno = Environment.GetEnvironmentVariable(VariableEntorno);
            if (!string.IsNullOrWhiteSpace(deEntorno)) return deEntorno;

            return ConfigurationManager.ConnectionStrings["GymDbConnection"]?.ConnectionString
                   ?? throw new ConfigurationErrorsException(
                       $"No se encontro 'GymDbConnection' en App.config ni la variable de entorno {VariableEntorno}.");
        }
    }

    public static SqlConnection ObtenerConexion() => new SqlConnection(CadenaConexion);

    public static bool ProbarConexion() => ProbarConexion(out _);

    /// <summary>Prueba la conexion y devuelve el mensaje real de la excepcion para diagnostico.</summary>
    public static bool ProbarConexion(out string? error)
    {
        try
        {
            using var cn = ObtenerConexion();
            cn.Open();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
