using Microsoft.Data.SqlClient;

namespace App.BLL;

/// <summary>
/// Convierte excepciones tecnicas (SQL Server, red, permisos) en mensajes que un usuario
/// de recepcion pueda entender. El detalle tecnico NO se muestra: va al log con un codigo.
/// </summary>
public static class ErrorTranslator
{
    /// <summary>True si la excepcion es una regla de negocio esperada (no un fallo del sistema).</summary>
    public static bool EsEsperado(Exception ex) =>
        ex is InvalidOperationException or UnauthorizedAccessException or ArgumentException;

    public static string Mensaje(Exception ex)
    {
        if (ex is AggregateException agg && agg.InnerException != null) ex = agg.InnerException;

        return ex switch
        {
            SqlException sql => TraducirSql(sql),
            UnauthorizedAccessException => ex.Message,
            InvalidOperationException => ex.Message,   // reglas de negocio: ya vienen redactadas para el usuario
            ArgumentException => ex.Message,
            TimeoutException => "La operacion tardo demasiado. Intente de nuevo.",
            _ => "Ocurrio un error inesperado."
        };
    }

    private static string TraducirSql(SqlException ex)
    {
        switch (ex.Number)
        {
            case 2627:
            case 2601:
                return "Ya existe un registro con esos datos (valor duplicado).";
            case 547:
                return ex.Message.Contains("DELETE", StringComparison.OrdinalIgnoreCase)
                    ? "No se puede eliminar porque tiene registros relacionados (por ejemplo inscripciones o pagos)."
                    : "La operacion hace referencia a un registro que no existe o no es valido.";
            case 515:
                return "Falta un dato obligatorio.";
            case 8152:
            case 2628:
                return "Uno de los textos excede la longitud permitida.";
            case 1205:
                return "Otro usuario estaba modificando los mismos datos. Intente de nuevo.";
            case 208:
            case 207:
                return "La base de datos no esta actualizada. Ejecute Database/mejoras_v9.sql en SQL Server.";
            case -2:
                return "La base de datos tardo demasiado en responder. Intente de nuevo.";
            case 53:
            case 4060:
            case 18456:
            case -1:
            case 2:
            case 233:
                return "No se pudo conectar con la base de datos. Verifique que SQL Server este activo y la cadena de conexion en App.config.";
            default:
                return "Ocurrio un error al acceder a la base de datos.";
        }
    }
}
