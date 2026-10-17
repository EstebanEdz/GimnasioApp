using Microsoft.Data.SqlClient;

namespace App.DAL;

public class UsuarioRow
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public int PerfilId { get; set; }
    public string PerfilNombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string PreguntaSeguridad { get; set; } = string.Empty;
    public string RespuestaSeguridadHash { get; set; } = string.Empty;

    // v9: seguridad de la cuenta
    public int IntentosFallidos { get; set; }
    public DateTime? BloqueadoHasta { get; set; }
    public bool EstaBloqueado { get; set; }      // calculado en SQL con la hora del servidor
    public DateTime? UltimoAcceso { get; set; }
}

public class UsuarioDAL
{
    private const string SelectBase = @"
        SELECT u.Id, u.NombreUsuario, u.ContrasenaHash, u.PerfilId, p.Nombre AS PerfilNombre, u.Activo,
               u.PreguntaSeguridad, u.RespuestaSeguridadHash,
               u.IntentosFallidos, u.BloqueadoHasta,
               CASE WHEN u.BloqueadoHasta IS NOT NULL AND u.BloqueadoHasta > SYSDATETIME() THEN 1 ELSE 0 END,
               u.UltimoAcceso
        FROM dbo.Usuarios u
        INNER JOIN dbo.Perfiles p ON p.Id = u.PerfilId";

    private static UsuarioRow Mapear(SqlDataReader r) => new UsuarioRow
    {
        Id = r.GetInt32(0),
        NombreUsuario = r.GetString(1),
        ContrasenaHash = r.GetString(2),
        PerfilId = r.GetInt32(3),
        PerfilNombre = r.GetString(4),
        Activo = r.GetBoolean(5),
        PreguntaSeguridad = r.IsDBNull(6) ? string.Empty : r.GetString(6),
        RespuestaSeguridadHash = r.IsDBNull(7) ? string.Empty : r.GetString(7),
        IntentosFallidos = r.GetInt32(8),
        BloqueadoHasta = r.IsDBNull(9) ? null : r.GetDateTime(9),
        EstaBloqueado = r.GetInt32(10) == 1,
        UltimoAcceso = r.IsDBNull(11) ? null : r.GetDateTime(11)
    };

    public UsuarioRow? ObtenerPorNombreUsuario(string nombreUsuario)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(SelectBase + " WHERE u.NombreUsuario = @u", cn);
            cmd.Parameters.AddWithValue("@u", nombreUsuario);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    public List<UsuarioRow> ObtenerTodos()
    {
        var lista = new List<UsuarioRow>();
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(SelectBase + " ORDER BY u.Id", cn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
        return lista;
    }

    public bool ExisteNombreUsuario(string nombreUsuario, int idExcluir = 0)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuarios WHERE NombreUsuario = @u AND Id <> @id", cn);
            cmd.Parameters.AddWithValue("@u", nombreUsuario);
            cmd.Parameters.AddWithValue("@id", idExcluir);
            return (int)cmd.ExecuteScalar() > 0;
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    public int Insertar(UsuarioRow u)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Usuarios (NombreUsuario, ContrasenaHash, PerfilId, Activo, PreguntaSeguridad, RespuestaSeguridadHash)
                OUTPUT INSERTED.Id
                VALUES (@u, @h, @p, @a, @preg, @resp)", cn);
            cmd.Parameters.AddWithValue("@u", u.NombreUsuario);
            cmd.Parameters.AddWithValue("@h", u.ContrasenaHash);
            cmd.Parameters.AddWithValue("@p", u.PerfilId);
            cmd.Parameters.AddWithValue("@a", u.Activo);
            cmd.Parameters.AddWithValue("@preg", (object?)u.PreguntaSeguridad ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@resp", (object?)u.RespuestaSeguridadHash ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>
    /// actualizarContrasena: tambien reescribe ContrasenaHash.
    /// actualizarSeguridad: tambien reescribe PreguntaSeguridad/RespuestaSeguridadHash
    /// (se deja aparte porque no siempre se cambian juntas).
    /// </summary>
    public void Actualizar(UsuarioRow u, bool actualizarContrasena, bool actualizarSeguridad = false)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            var set = new List<string> { "NombreUsuario=@u", "PerfilId=@p", "Activo=@a" };
            if (actualizarContrasena) set.Add("ContrasenaHash=@h");
            if (actualizarSeguridad) set.Add("PreguntaSeguridad=@preg");
            if (actualizarSeguridad) set.Add("RespuestaSeguridadHash=@resp");

            string sql = $"UPDATE dbo.Usuarios SET {string.Join(", ", set)} WHERE Id=@id";
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u", u.NombreUsuario);
            if (actualizarContrasena) cmd.Parameters.AddWithValue("@h", u.ContrasenaHash);
            if (actualizarSeguridad)
            {
                cmd.Parameters.AddWithValue("@preg", (object?)u.PreguntaSeguridad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@resp", (object?)u.RespuestaSeguridadHash ?? DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@p", u.PerfilId);
            cmd.Parameters.AddWithValue("@a", u.Activo);
            cmd.Parameters.AddWithValue("@id", u.Id);
            cmd.ExecuteNonQuery();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>Cambia unicamente el hash de la contraseña (usado por el flujo de recuperacion).</summary>
    public void CambiarContrasena(string nombreUsuario, string nuevaContrasenaHash)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("UPDATE dbo.Usuarios SET ContrasenaHash=@h WHERE NombreUsuario=@u", cn);
            cmd.Parameters.AddWithValue("@h", nuevaContrasenaHash);
            cmd.Parameters.AddWithValue("@u", nombreUsuario);
            cmd.ExecuteNonQuery();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>
    /// Suma un intento fallido y, si se alcanza el maximo, bloquea la cuenta N minutos.
    /// Devuelve los intentos acumulados y si la cuenta quedo bloqueada.
    /// (En un UPDATE de T-SQL, las expresiones de la derecha usan el valor ANTERIOR de la columna.)
    /// </summary>
    public (int intentos, bool bloqueada) RegistrarIntentoFallido(int id, int maxIntentos, int minutosBloqueo)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Usuarios
                SET IntentosFallidos = IntentosFallidos + 1,
                    BloqueadoHasta = CASE WHEN IntentosFallidos + 1 >= @max
                                          THEN DATEADD(MINUTE, @min, SYSDATETIME())
                                          ELSE BloqueadoHasta END
                WHERE Id = @id;
                SELECT IntentosFallidos,
                       CASE WHEN BloqueadoHasta IS NOT NULL AND BloqueadoHasta > SYSDATETIME() THEN 1 ELSE 0 END
                FROM dbo.Usuarios WHERE Id = @id;", cn);
            cmd.Parameters.AddWithValue("@max", maxIntentos);
            cmd.Parameters.AddWithValue("@min", minutosBloqueo);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? (reader.GetInt32(0), reader.GetInt32(1) == 1) : (0, false);
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>Login correcto: limpia intentos/bloqueo y guarda la fecha del ultimo acceso.</summary>
    public void RegistrarAccesoExitoso(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Usuarios
                SET IntentosFallidos = 0, BloqueadoHasta = NULL, UltimoAcceso = SYSDATETIME()
                WHERE Id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>Desbloqueo manual por parte de un administrador.</summary>
    public void Desbloquear(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Usuarios SET IntentosFallidos = 0, BloqueadoHasta = NULL WHERE Id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    /// <summary>Cuantos administradores activos hay, sin contar al usuario indicado (para no dejar el sistema sin admin).</summary>
    public int ContarAdministradoresActivos(int idExcluir)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM dbo.Usuarios u
                INNER JOIN dbo.Perfiles p ON p.Id = u.PerfilId
                WHERE p.Nombre = N'Administrador' AND u.Activo = 1 AND u.Id <> @id", cn);
            cmd.Parameters.AddWithValue("@id", idExcluir);
            return (int)cmd.ExecuteScalar();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    public UsuarioRow? ObtenerPorId(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand(SelectBase + " WHERE u.Id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }

    public void Eliminar(int id)
    {
        using var cn = Conexion.ObtenerConexion();
        try
        {
            cn.Open();
            using var cmd = new SqlCommand("DELETE FROM dbo.Usuarios WHERE Id=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }
}
