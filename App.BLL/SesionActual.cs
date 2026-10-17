using App.BLL.Entidades;

namespace App.BLL;

/// <summary>Codigos de permiso (deben coincidir con la columna Permisos.Codigo de la BD).</summary>
public static class Permisos
{
    public const string MiembrosVer = "MIEMBROS_VER";
    public const string MiembrosEditar = "MIEMBROS_EDITAR";
    public const string PlanesVer = "PLANES_VER";
    public const string PlanesEditar = "PLANES_EDITAR";
    public const string EntrenadoresVer = "ENTRENADORES_VER";
    public const string EntrenadoresEditar = "ENTRENADORES_EDITAR";
    public const string InscripcionesVer = "INSCRIPCIONES_VER";
    public const string InscripcionesEditar = "INSCRIPCIONES_EDITAR";
    public const string MembresiasVer = "MEMBRESIAS_VER";
    public const string MembresiasGestionar = "MEMBRESIAS_GESTIONAR";
    public const string PagosVer = "PAGOS_VER";
    public const string PagosRegistrar = "PAGOS_REGISTRAR";
    public const string PagosAnular = "PAGOS_ANULAR";
    public const string ReportesBasicos = "REPORTES_BASICOS";
    public const string ReportesFinancieros = "REPORTES_FINANCIEROS";
    public const string UsuariosAdmin = "USUARIOS_ADMIN";
    public const string PermisosAdmin = "PERMISOS_ADMIN";
    public const string AuditoriaVer = "AUDITORIA_VER";
}

/// <summary>
/// Usuario que inicio sesion y los permisos de su perfil (se cargan una vez en el login).
/// Los servicios de la BLL llaman a Exigir(...) para que la regla se cumpla aunque alguien
/// llame al servicio saltandose la interfaz (defensa en profundidad: no depende solo de ocultar botones).
/// </summary>
public static class SesionActual
{
    private static HashSet<string> _permisos = new(StringComparer.OrdinalIgnoreCase);

    public static Usuario? Usuario { get; private set; }

    public static string NombreUsuario => Usuario?.NombreUsuario ?? "sistema";

    public static int? UsuarioId => Usuario is { Id: > 0 } u ? u.Id : null;

    public static void Iniciar(Usuario usuario, IEnumerable<string> permisos)
    {
        Usuario = usuario;
        _permisos = new HashSet<string>(permisos, StringComparer.OrdinalIgnoreCase);
    }

    public static void Cerrar()
    {
        Usuario = null;
        _permisos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public static bool Tiene(string codigoPermiso) => _permisos.Contains(codigoPermiso);

    /// <summary>Lanza UnauthorizedAccessException si el usuario actual no tiene el permiso.</summary>
    public static void Exigir(string codigoPermiso)
    {
        if (!Tiene(codigoPermiso))
            throw new UnauthorizedAccessException("Su perfil no tiene permiso para realizar esta accion.");
    }
}
