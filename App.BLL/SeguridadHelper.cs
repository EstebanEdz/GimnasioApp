using System.Security.Cryptography;
using System.Text;

namespace App.BLL;

/// <summary>
/// Utilidades de seguridad: hashing de contraseñas de usuarios de la app (nunca texto plano)
/// y politica de contraseñas.
///
/// v9: las contraseñas nuevas se guardan con PBKDF2 (salt aleatorio + 100.000 iteraciones),
/// formato "PBKDF2$iteraciones$salt$hash". Los hashes SHA256 de versiones anteriores siguen
/// funcionando y se convierten solos a PBKDF2 en el siguiente login exitoso.
/// </summary>
public static class SeguridadHelper
{
    private const string Prefijo = "PBKDF2";
    private const int Iteraciones = 100_000;
    private const int LongitudHash = 32;

    /// <summary>Hash seguro para guardar: PBKDF2-SHA256 con salt aleatorio unico por contraseña.</summary>
    public static string HashearContrasena(string textoPlano)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(textoPlano, salt, Iteraciones, HashAlgorithmName.SHA256, LongitudHash);
        return $"{Prefijo}${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>True si el hash guardado es del formato antiguo (SHA256 sin salt) y conviene actualizarlo.</summary>
    public static bool EsHashAntiguo(string hashGuardado) =>
        !hashGuardado.StartsWith(Prefijo + "$", StringComparison.Ordinal);

    /// <summary>
    /// Politica de contraseñas para las NUEVAS contraseñas: minimo 8 caracteres, con al menos
    /// una letra y un numero. Devuelve el mensaje de error o null si cumple.
    /// </summary>
    public static string? ValidarFortaleza(string? contrasena)
    {
        if (string.IsNullOrEmpty(contrasena) || contrasena.Length < 8)
            return "La contraseña debe tener al menos 8 caracteres.";
        if (!contrasena.Any(char.IsLetter) || !contrasena.Any(char.IsDigit))
            return "La contraseña debe combinar letras y numeros.";
        return null;
    }

    /// <summary>Calcula el hash SHA256 (hex, minusculas). Solo se conserva para compatibilidad con hashes antiguos.</summary>
    public static string CalcularHash(string textoPlano)
    {
        using var sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
        var sb = new StringBuilder();
        foreach (byte b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>Verifica una contraseña contra el hash guardado (formato nuevo PBKDF2 o SHA256 antiguo).</summary>
    public static bool Verificar(string textoPlano, string hashGuardado)
    {
        if (string.IsNullOrEmpty(hashGuardado)) return false;

        if (EsHashAntiguo(hashGuardado))
            return string.Equals(CalcularHash(textoPlano), hashGuardado, StringComparison.OrdinalIgnoreCase);

        string[] partes = hashGuardado.Split('$');
        if (partes.Length != 4 || !int.TryParse(partes[1], out int iteraciones) || iteraciones <= 0) return false;

        byte[] salt, esperado;
        try
        {
            salt = Convert.FromBase64String(partes[2]);
            esperado = Convert.FromBase64String(partes[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(textoPlano, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
        // Comparacion en tiempo constante: evita adivinar el hash midiendo cuanto tarda la comparacion.
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}
