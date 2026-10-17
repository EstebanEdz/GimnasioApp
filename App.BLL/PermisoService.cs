using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public class PermisoService
{
    private readonly PermisoDAL _dal = new();
    private readonly PerfilDAL _perfilDal = new();
    private readonly AuditoriaService _auditoria = new();

    /// <summary>Permisos que SIEMPRE conserva el perfil Administrador, para que nadie se quede sin poder administrar el sistema.</summary>
    private static readonly string[] PermisosProtegidosAdmin = { Permisos.UsuariosAdmin, Permisos.PermisosAdmin };

    public List<Permiso> ObtenerTodos() =>
        _dal.ObtenerTodos().Select(r => new Permiso
        {
            Id = r.Id,
            Codigo = r.Codigo,
            Descripcion = r.Descripcion,
            Modulo = r.Modulo
        }).ToList();

    /// <summary>Se usa en el login (sin sesion todavia), por eso no exige permiso.</summary>
    public List<string> ObtenerCodigosPorPerfil(int perfilId) => _dal.ObtenerCodigosPorPerfil(perfilId);

    public void Guardar(int perfilId, IEnumerable<string> codigos)
    {
        SesionActual.Exigir(Permisos.PermisosAdmin);

        var lista = codigos.ToList();
        var perfil = _perfilDal.ObtenerTodos().FirstOrDefault(p => p.Id == perfilId)
                     ?? throw new InvalidOperationException("El perfil no existe.");

        if (perfil.Nombre == "Administrador")
        {
            foreach (string protegido in PermisosProtegidosAdmin)
                if (!lista.Contains(protegido)) lista.Add(protegido);
        }

        _dal.GuardarPermisosDePerfil(perfilId, lista);
        _auditoria.Registrar("PERMISOS_ACTUALIZADOS", "Perfil", perfilId,
            $"Perfil {perfil.Nombre}: {lista.Count} permisos asignados");
    }
}
