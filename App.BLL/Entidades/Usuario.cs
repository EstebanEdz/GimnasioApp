namespace App.BLL.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public int PerfilId { get; set; }
    public string PerfilNombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public string PreguntaSeguridad { get; set; } = string.Empty;
    public string RespuestaSeguridadHash { get; set; } = string.Empty;

    public bool Bloqueado { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    public TipoPerfil Perfil => (TipoPerfil)PerfilId;

    public override string ToString() => NombreUsuario;
}
