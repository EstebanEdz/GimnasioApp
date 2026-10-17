namespace App.DAL;

public class AuditoriaRow
{
    public long Id { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string EntidadId { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
}

public class LogErrorRow
{
    public long Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contexto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}

public class AuditoriaDAL
{
    public void Insertar(int? usuarioId, string nombreUsuario, string accion, string entidad, string? entidadId, string? detalle)
    {
        DbHelper.Ejecutar(@"
            INSERT INTO dbo.Auditoria (UsuarioId, NombreUsuario, Accion, Entidad, EntidadId, Detalle)
            VALUES (@uid, @user, @accion, @entidad, @eid, @detalle)",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@uid", DbHelper.ONulo(usuarioId));
                cmd.Parameters.AddWithValue("@user", nombreUsuario);
                cmd.Parameters.AddWithValue("@accion", accion);
                cmd.Parameters.AddWithValue("@entidad", entidad);
                cmd.Parameters.AddWithValue("@eid", DbHelper.ONulo(entidadId));
                cmd.Parameters.AddWithValue("@detalle", DbHelper.ONulo(detalle));
            });
    }

    public List<AuditoriaRow> Consultar(DateTime desde, DateTime hasta, string? usuario, string? accion)
    {
        return DbHelper.Consultar(@"
            SELECT TOP (1000) Id, Fecha, NombreUsuario, Accion, Entidad, ISNULL(EntidadId, ''), ISNULL(Detalle, '')
            FROM dbo.Auditoria
            WHERE Fecha >= @desde AND Fecha < @hasta
              AND (@usuario IS NULL OR NombreUsuario LIKE '%' + @usuario + '%')
              AND (@accion IS NULL OR Accion = @accion)
            ORDER BY Fecha DESC, Id DESC",
            r => new AuditoriaRow
            {
                Id = r.GetInt64(0),
                Fecha = r.GetDateTime(1),
                NombreUsuario = r.GetString(2),
                Accion = r.GetString(3),
                Entidad = r.GetString(4),
                EntidadId = r.GetString(5),
                Detalle = r.GetString(6)
            },
            cmd =>
            {
                cmd.Parameters.AddWithValue("@desde", desde.Date);
                cmd.Parameters.AddWithValue("@hasta", hasta.Date.AddDays(1));   // incluye todo el dia "hasta"
                cmd.Parameters.AddWithValue("@usuario", DbHelper.ONulo(usuario));
                cmd.Parameters.AddWithValue("@accion", DbHelper.ONulo(accion));
            });
    }

    public List<string> ObtenerAccionesDistintas() => DbHelper.Consultar(
        "SELECT DISTINCT Accion FROM dbo.Auditoria ORDER BY Accion", r => r.GetString(0));

    public void InsertarError(string codigo, string? usuario, string? contexto, string mensaje, string? stackTrace)
    {
        DbHelper.Ejecutar(@"
            INSERT INTO dbo.LogErrores (Codigo, NombreUsuario, Contexto, Mensaje, StackTrace)
            VALUES (@codigo, @user, @ctx, @msg, @stack)",
            cmd =>
            {
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cmd.Parameters.AddWithValue("@user", DbHelper.ONulo(usuario));
                cmd.Parameters.AddWithValue("@ctx", DbHelper.ONulo(contexto));
                cmd.Parameters.AddWithValue("@msg", mensaje);
                cmd.Parameters.AddWithValue("@stack", DbHelper.ONulo(stackTrace));
            });
    }

    public List<LogErrorRow> ObtenerUltimosErrores(int cantidad = 200) => DbHelper.Consultar(@"
        SELECT TOP (@n) Id, Codigo, Fecha, ISNULL(NombreUsuario, ''), ISNULL(Contexto, ''), Mensaje
        FROM dbo.LogErrores ORDER BY Id DESC",
        r => new LogErrorRow
        {
            Id = r.GetInt64(0),
            Codigo = r.GetString(1),
            Fecha = r.GetDateTime(2),
            NombreUsuario = r.GetString(3),
            Contexto = r.GetString(4),
            Mensaje = r.GetString(5)
        },
        cmd => cmd.Parameters.AddWithValue("@n", cantidad));
}
