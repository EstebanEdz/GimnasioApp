namespace App.BLL.Entidades;

public enum TipoPerfil
{
    Administrador = 1,
    Operador = 2,
    Consulta = 3
}

public class Perfil
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public override string ToString() => Nombre;
}
