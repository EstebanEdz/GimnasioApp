using App.DAL;

namespace App.BLL;

/// <summary>
/// Registro centralizado de errores inesperados. Cada error recibe un codigo corto que se le
/// muestra al usuario ("Codigo de soporte: A1B2C3D4"); con ese codigo se busca el detalle
/// tecnico en la tabla LogErrores o en la carpeta Logs. Registrar() NUNCA lanza excepciones.
/// </summary>
public static class ManejadorErrores
{
    private static readonly object _candado = new();

    public static string Registrar(Exception ex, string contexto)
    {
        string codigo = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        string usuario = SesionActual.NombreUsuario;

        // 1) Archivo local: funciona incluso cuando la base de datos es el problema.
        try
        {
            string carpeta = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(carpeta);
            string archivo = Path.Combine(carpeta, $"errores-{DateTime.Now:yyyyMMdd}.log");
            string linea = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {codigo} | {usuario} | {contexto}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}";
            lock (_candado) File.AppendAllText(archivo, linea);
        }
        catch
        {
            // Si ni el archivo se puede escribir, no hay nada mas que hacer: el error original es el importante.
        }

        // 2) Base de datos (para consultarlo desde la app en Seguridad > Auditoria y errores).
        try
        {
            string mensaje = ex.Message.Length > 500 ? ex.Message[..500] : ex.Message;
            new AuditoriaDAL().InsertarError(codigo, usuario, contexto, mensaje, ex.ToString());
        }
        catch
        {
        }

        return codigo;
    }

    /// <summary>
    /// Devuelve el texto que se le muestra al usuario. Las reglas de negocio esperadas no se
    /// registran como error; los fallos del sistema si, y agregan el codigo de soporte.
    /// </summary>
    public static string MensajeParaUsuario(Exception ex, string contexto)
    {
        string amigable = ErrorTranslator.Mensaje(ex);
        if (ErrorTranslator.EsEsperado(ex)) return amigable;

        string codigo = Registrar(ex, contexto);
        return $"{amigable}\n\nCodigo de soporte: {codigo}";
    }
}
