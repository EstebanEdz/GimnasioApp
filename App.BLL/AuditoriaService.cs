using App.DAL;

namespace App.BLL;

public class RegistroAuditoria
{
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string EntidadId { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
}

public class RegistroError
{
    public string Codigo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Contexto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}

/// <summary>
/// Bitacora de acciones importantes (quien hizo que y cuando). Registrar() esta pensado para
/// llamarse DESPUES de que la operacion principal salio bien, y nunca debe romperla:
/// si la bitacora falla, el error se manda al log pero la operacion del usuario sigue valida.
/// </summary>
public class AuditoriaService
{
    private readonly AuditoriaDAL _dal = new();

    public void Registrar(string accion, string entidad, object? entidadId = null, string? detalle = null)
        => RegistrarComo(SesionActual.UsuarioId, SesionActual.NombreUsuario, accion, entidad, entidadId, detalle);

    /// <summary>Version con usuario explicito (para el login, donde aun no hay sesion).</summary>
    public void RegistrarComo(int? usuarioId, string nombreUsuario, string accion, string entidad, object? entidadId = null, string? detalle = null)
    {
        try
        {
            if (detalle != null && detalle.Length > 500) detalle = detalle[..500];
            _dal.Insertar(usuarioId, nombreUsuario, accion, entidad, entidadId?.ToString(), detalle);
        }
        catch (Exception ex)
        {
            ManejadorErrores.Registrar(ex, "Auditoria.Registrar");
        }
    }

    public List<RegistroAuditoria> Consultar(DateTime desde, DateTime hasta, string? usuario, string? accion)
    {
        SesionActual.Exigir(Permisos.AuditoriaVer);
        return _dal.Consultar(desde, hasta,
                string.IsNullOrWhiteSpace(usuario) ? null : usuario.Trim(),
                string.IsNullOrWhiteSpace(accion) ? null : accion)
            .Select(r => new RegistroAuditoria
            {
                Fecha = r.Fecha,
                Usuario = r.NombreUsuario,
                Accion = r.Accion,
                Entidad = r.Entidad,
                EntidadId = r.EntidadId,
                Detalle = r.Detalle
            }).ToList();
    }

    public List<string> ObtenerAcciones()
    {
        SesionActual.Exigir(Permisos.AuditoriaVer);
        return _dal.ObtenerAccionesDistintas();
    }

    public List<RegistroError> ObtenerUltimosErrores()
    {
        SesionActual.Exigir(Permisos.AuditoriaVer);
        return _dal.ObtenerUltimosErrores().Select(e => new RegistroError
        {
            Codigo = e.Codigo,
            Fecha = e.Fecha,
            Usuario = e.NombreUsuario,
            Contexto = e.Contexto,
            Mensaje = e.Mensaje
        }).ToList();
    }
}
