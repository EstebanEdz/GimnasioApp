using App.BLL.Entidades;
using App.DAL;

namespace App.BLL;

public enum ResultadoLogin
{
    Ok,
    CredencialesInvalidas,
    UsuarioInactivo,
    CuentaBloqueada
}

/// <summary>Resultado de un intento de login: el motivo, el usuario (si entro) y un mensaje listo para mostrar.</summary>
public record ResultadoAutenticacion(ResultadoLogin Resultado, Usuario? Usuario, string Mensaje);

public class UsuarioService
{
    private readonly UsuarioDAL _dal = new();
    private readonly PerfilDAL _perfilDal = new();
    private readonly PermisoService _permisoService = new();
    private readonly AuditoriaService _auditoria = new();

    /// <summary>Envuelve la verificacion de conectividad del DAL para que la UI nunca llame a App.DAL directamente.</summary>
    public bool ProbarConexion() => Conexion.ProbarConexion();

    /// <summary>Igual que ProbarConexion(), pero además devuelve el mensaje real de la excepción para diagnóstico en la UI.</summary>
    public bool ProbarConexion(out string? error) => Conexion.ProbarConexion(out error);

    /// <summary>Version simple (compatibilidad): devuelve el usuario o null.</summary>
    public Usuario? Autenticar(string nombreUsuario, string contrasenaPlano) =>
        AutenticarDetallado(nombreUsuario, contrasenaPlano).Usuario;

    /// <summary>
    /// Login completo. Verifica el hash (nunca texto plano), cuenta los intentos fallidos y bloquea
    /// la cuenta al llegar al maximo, actualiza hashes antiguos a PBKDF2, carga los permisos del
    /// perfil en SesionActual y deja todo registrado en la auditoria.
    /// </summary>
    public ResultadoAutenticacion AutenticarDetallado(string nombreUsuario, string contrasenaPlano)
    {
        var row = _dal.ObtenerPorNombreUsuario(nombreUsuario);

        if (row == null)
        {
            _auditoria.RegistrarComo(null, nombreUsuario, "LOGIN_FALLIDO", "Usuario", null, "Usuario inexistente");
            return new ResultadoAutenticacion(ResultadoLogin.CredencialesInvalidas, null, "Usuario o contraseña incorrectos.");
        }

        if (!row.Activo)
        {
            _auditoria.RegistrarComo(row.Id, row.NombreUsuario, "LOGIN_FALLIDO", "Usuario", row.Id, "Usuario inactivo");
            return new ResultadoAutenticacion(ResultadoLogin.UsuarioInactivo, null,
                "Este usuario esta inactivo. Contacte al administrador.");
        }

        if (row.EstaBloqueado)
        {
            _auditoria.RegistrarComo(row.Id, row.NombreUsuario, "LOGIN_BLOQUEADO", "Usuario", row.Id, "Intento con la cuenta bloqueada");
            return new ResultadoAutenticacion(ResultadoLogin.CuentaBloqueada, null,
                $"La cuenta esta bloqueada temporalmente por demasiados intentos fallidos. Intente de nuevo despues de las {row.BloqueadoHasta:HH:mm} o pida a un administrador que la desbloquee.");
        }

        if (!SeguridadHelper.Verificar(contrasenaPlano, row.ContrasenaHash))
        {
            var (intentos, bloqueada) = _dal.RegistrarIntentoFallido(row.Id, Configuracion.IntentosParaBloqueo, Configuracion.MinutosBloqueo);
            _auditoria.RegistrarComo(row.Id, row.NombreUsuario, "LOGIN_FALLIDO", "Usuario", row.Id,
                $"Contraseña incorrecta (intento {intentos})");

            if (bloqueada)
            {
                _auditoria.RegistrarComo(row.Id, row.NombreUsuario, "CUENTA_BLOQUEADA", "Usuario", row.Id,
                    $"Bloqueada {Configuracion.MinutosBloqueo} min por {intentos} intentos fallidos");
                return new ResultadoAutenticacion(ResultadoLogin.CuentaBloqueada, null,
                    $"Demasiados intentos fallidos: la cuenta quedo bloqueada por {Configuracion.MinutosBloqueo} minutos.");
            }

            return new ResultadoAutenticacion(ResultadoLogin.CredencialesInvalidas, null, "Usuario o contraseña incorrectos.");
        }

        // Login correcto
        _dal.RegistrarAccesoExitoso(row.Id);

        // Si la contraseña estaba con el hash antiguo (SHA256 sin salt), la guardamos de nuevo con PBKDF2.
        if (SeguridadHelper.EsHashAntiguo(row.ContrasenaHash))
        {
            _dal.CambiarContrasena(row.NombreUsuario, SeguridadHelper.HashearContrasena(contrasenaPlano));
        }

        var usuario = Mapear(row);
        usuario.ContrasenaHash = string.Empty;          // la sesion no necesita (ni debe guardar) los hashes
        usuario.RespuestaSeguridadHash = string.Empty;
        SesionActual.Iniciar(usuario, _permisoService.ObtenerCodigosPorPerfil(usuario.PerfilId));
        _auditoria.Registrar("LOGIN_OK", "Usuario", usuario.Id, $"Perfil {usuario.PerfilNombre}");
        return new ResultadoAutenticacion(ResultadoLogin.Ok, usuario, string.Empty);
    }

    /// <summary>Cambio de contraseña del propio usuario: exige la contraseña actual y cumple la politica.</summary>
    public List<string> CambiarContrasenaPropia(string nombreUsuario, string actual, string nueva)
    {
        var errores = new List<string>();
        var row = _dal.ObtenerPorNombreUsuario(nombreUsuario);

        if (row == null || !SeguridadHelper.Verificar(actual, row.ContrasenaHash))
            errores.Add("La contraseña actual no es correcta.");

        errores.AgregarSiHayError(SeguridadHelper.ValidarFortaleza(nueva));
        if (errores.Count == 0 && actual == nueva)
            errores.Add("La nueva contraseña debe ser distinta a la actual.");
        if (errores.Count > 0) return errores;

        _dal.CambiarContrasena(nombreUsuario, SeguridadHelper.HashearContrasena(nueva));
        _auditoria.Registrar("CONTRASENA_CAMBIADA", "Usuario", row!.Id, "El usuario cambio su propia contraseña");
        return errores;
    }

    public void Desbloquear(int id)
    {
        SesionActual.Exigir(Permisos.UsuariosAdmin);
        _dal.Desbloquear(id);
        _auditoria.Registrar("USUARIO_DESBLOQUEADO", "Usuario", id, null);
    }

    public List<Perfil> ObtenerPerfiles()
    {
        var lista = new List<Perfil>();
        foreach (var p in _perfilDal.ObtenerTodos())
            lista.Add(new Perfil { Id = p.Id, Nombre = p.Nombre });
        return lista;
    }

    public List<Usuario> ObtenerTodos()
    {
        SesionActual.Exigir(Permisos.UsuariosAdmin);
        var lista = new List<Usuario>();
        foreach (var r in _dal.ObtenerTodos())
            lista.Add(Mapear(r));
        return lista;
    }

    public List<string> Validar(Usuario u, string? contrasenaPlanoNueva)
    {
        var errores = new List<string>();
        if (string.IsNullOrWhiteSpace(u.NombreUsuario))
            errores.Add("El nombre de usuario es obligatorio.");
        else if (_dal.ExisteNombreUsuario(u.NombreUsuario, u.Id))
            errores.Add("Ya existe un usuario con ese nombre.");

        if (u.Id == 0 && string.IsNullOrWhiteSpace(contrasenaPlanoNueva))
            errores.Add("La contraseña es obligatoria para un usuario nuevo.");

        if (!string.IsNullOrEmpty(contrasenaPlanoNueva))
            errores.AgregarSiHayError(SeguridadHelper.ValidarFortaleza(contrasenaPlanoNueva));

        if (u.PerfilId <= 0)
            errores.Add("Seleccione un perfil.");

        // Regla de negocio: siempre debe quedar al menos un administrador activo.
        if (u.Id > 0)
        {
            var existente = _dal.ObtenerPorId(u.Id);
            bool eraAdminActivo = existente != null && existente.PerfilNombre == "Administrador" && existente.Activo;
            bool seguiraAdminActivo = u.PerfilId == (int)TipoPerfil.Administrador && u.Activo;
            if (eraAdminActivo && !seguiraAdminActivo && _dal.ContarAdministradoresActivos(u.Id) == 0)
                errores.Add("Debe existir al menos un administrador activo; no puede cambiar el perfil ni desactivar a este usuario.");
        }

        return errores;
    }

    public void Guardar(Usuario u, string? contrasenaPlanoNueva, string? preguntaSeguridadNueva = null, string? respuestaSeguridadNuevaPlano = null)
    {
        SesionActual.Exigir(Permisos.UsuariosAdmin);
        bool esNuevo = u.Id == 0;
        bool actualizarContrasena = !string.IsNullOrEmpty(contrasenaPlanoNueva);
        bool actualizarSeguridad = !string.IsNullOrWhiteSpace(preguntaSeguridadNueva) && !string.IsNullOrWhiteSpace(respuestaSeguridadNuevaPlano);

        var row = new UsuarioRow
        {
            Id = u.Id,
            NombreUsuario = u.NombreUsuario.Trim(),
            ContrasenaHash = actualizarContrasena ? SeguridadHelper.HashearContrasena(contrasenaPlanoNueva!) : u.ContrasenaHash,
            PerfilId = u.PerfilId,
            Activo = u.Activo,
            PreguntaSeguridad = actualizarSeguridad ? preguntaSeguridadNueva!.Trim() : u.PreguntaSeguridad,
            RespuestaSeguridadHash = actualizarSeguridad
                ? SeguridadHelper.HashearContrasena(NormalizarRespuesta(respuestaSeguridadNuevaPlano!))
                : u.RespuestaSeguridadHash
        };

        if (u.Id > 0)
        {
            _dal.Actualizar(row, actualizarContrasena, actualizarSeguridad);
        }
        else
        {
            // Un usuario nuevo siempre necesita pregunta/respuesta de seguridad, aunque no
            // se hayan marcado como "actualizar" (para poder recuperar la contraseña despues).
            if (!actualizarSeguridad)
            {
                row.PreguntaSeguridad = preguntaSeguridadNueva?.Trim() ?? string.Empty;
                row.RespuestaSeguridadHash = string.IsNullOrWhiteSpace(respuestaSeguridadNuevaPlano)
                    ? string.Empty
                    : SeguridadHelper.HashearContrasena(NormalizarRespuesta(respuestaSeguridadNuevaPlano));
            }
            u.Id = _dal.Insertar(row);
        }

        _auditoria.Registrar(esNuevo ? "USUARIO_CREADO" : "USUARIO_ACTUALIZADO", "Usuario", u.Id,
            $"{row.NombreUsuario} (perfil {u.PerfilNombre}){(actualizarContrasena ? ", contraseña cambiada" : string.Empty)}");
    }

    public void Eliminar(int id)
    {
        SesionActual.Exigir(Permisos.UsuariosAdmin);

        if (SesionActual.UsuarioId == id)
            throw new InvalidOperationException("No puede eliminar su propio usuario mientras tiene la sesion abierta.");

        var existente = _dal.ObtenerPorId(id) ?? throw new InvalidOperationException("El usuario no existe.");
        if (existente.PerfilNombre == "Administrador" && existente.Activo && _dal.ContarAdministradoresActivos(id) == 0)
            throw new InvalidOperationException("No puede eliminar al unico administrador activo.");

        _dal.Eliminar(id);
        _auditoria.Registrar("USUARIO_ELIMINADO", "Usuario", id, existente.NombreUsuario);
    }

    // ---------------- RECUPERACION DE CONTRASEÑA ----------------

    /// <summary>Devuelve la pregunta de seguridad del usuario, o null si el usuario no existe o no tiene una configurada.</summary>
    public string? ObtenerPreguntaSeguridad(string nombreUsuario)
    {
        var row = _dal.ObtenerPorNombreUsuario(nombreUsuario);
        if (row == null || string.IsNullOrWhiteSpace(row.PreguntaSeguridad)) return null;
        return row.PreguntaSeguridad;
    }

    /// <summary>Verifica la respuesta de seguridad (normalizada) contra el hash guardado.</summary>
    public bool VerificarRespuestaSeguridad(string nombreUsuario, string respuestaPlano)
    {
        var row = _dal.ObtenerPorNombreUsuario(nombreUsuario);
        if (row == null || string.IsNullOrWhiteSpace(row.RespuestaSeguridadHash)) return false;
        // Verificar() entiende tanto el hash nuevo (PBKDF2) como el SHA256 de las versiones anteriores.
        return SeguridadHelper.Verificar(NormalizarRespuesta(respuestaPlano), row.RespuestaSeguridadHash);
    }

    /// <summary>Cambia la contraseña de un usuario ya verificado (flujo de "olvide mi contraseña").</summary>
    public List<string> CambiarContrasena(string nombreUsuario, string nuevaContrasenaPlano)
    {
        var errores = new List<string>();
        errores.AgregarSiHayError(SeguridadHelper.ValidarFortaleza(nuevaContrasenaPlano));

        if (errores.Count > 0) return errores;

        _dal.CambiarContrasena(nombreUsuario, SeguridadHelper.HashearContrasena(nuevaContrasenaPlano));
        var row = _dal.ObtenerPorNombreUsuario(nombreUsuario);
        _auditoria.RegistrarComo(row?.Id, nombreUsuario, "CONTRASENA_RECUPERADA", "Usuario", row?.Id,
            "Contraseña restablecida con la pregunta de seguridad");
        return errores;
    }

    // La respuesta se normaliza (sin espacios extremos, minusculas) para que
    // "Bogota", "bogota " o "BOGOTA" verifiquen igual sin ser estrictos con el usuario.
    private static string NormalizarRespuesta(string respuesta) => respuesta.Trim().ToLowerInvariant();

    private static Usuario Mapear(UsuarioRow r) => new Usuario
    {
        Id = r.Id,
        NombreUsuario = r.NombreUsuario,
        ContrasenaHash = r.ContrasenaHash,
        PerfilId = r.PerfilId,
        PerfilNombre = r.PerfilNombre,
        Activo = r.Activo,
        PreguntaSeguridad = r.PreguntaSeguridad,
        RespuestaSeguridadHash = r.RespuestaSeguridadHash,
        Bloqueado = r.EstaBloqueado,
        UltimoAcceso = r.UltimoAcceso
    };
}
