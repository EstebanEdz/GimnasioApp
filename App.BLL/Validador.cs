using System.Net.Mail;
using System.Text.RegularExpressions;

namespace App.BLL;

/// <summary>
/// Reglas de validacion reutilizables (nombre, documento, telefono, correo).
/// Cada metodo devuelve el mensaje de error, o null si el valor es valido.
/// Asi MiembroService y EntrenadorService no repiten la misma logica.
/// </summary>
public static class Validador
{
    private static readonly Regex RegexNombre =
        new(@"^\p{L}[\p{L} .'\-]{2,99}$", RegexOptions.Compiled);

    private static readonly Regex RegexDocumento =
        new(@"^[0-9A-Za-z]{4,15}$", RegexOptions.Compiled);

    public static string? Nombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return "El nombre es obligatorio.";
        if (!RegexNombre.IsMatch(nombre.Trim()))
            return "El nombre solo puede tener letras, espacios, punto, guion o apostrofe (3 a 100 caracteres).";
        return null;
    }

    public static string? Documento(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento)) return "El documento es obligatorio.";
        if (!RegexDocumento.IsMatch(documento.Trim()))
            return "El documento debe tener entre 4 y 15 caracteres, solo letras y numeros.";
        return null;
    }

    public static string? Telefono(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono)) return "El telefono es obligatorio.";
        int digitos = telefono.Count(char.IsDigit);
        bool soloCaracteresValidos = telefono.All(c => char.IsDigit(c) || c is ' ' or '-' or '+' or '(' or ')');
        if (!soloCaracteresValidos || digitos < 7 || digitos > 15)
            return "El telefono debe tener entre 7 y 15 digitos (se permiten espacios, guiones, + y parentesis).";
        return null;
    }

    public static string? Correo(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return "El correo es obligatorio.";
        string limpio = correo.Trim();
        try
        {
            var direccion = new MailAddress(limpio);
            bool dominioValido = direccion.Address == limpio && direccion.Host.Contains('.');
            return dominioValido ? null : "El correo no es valido (ejemplo: nombre@dominio.com).";
        }
        catch (FormatException)
        {
            return "El correo no es valido (ejemplo: nombre@dominio.com).";
        }
    }

    /// <summary>Agrega el error a la lista solo si hay mensaje (null = valido).</summary>
    public static void AgregarSiHayError(this List<string> errores, string? mensaje)
    {
        if (mensaje != null) errores.Add(mensaje);
    }
}
