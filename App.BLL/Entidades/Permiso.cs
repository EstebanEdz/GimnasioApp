namespace App.BLL.Entidades;

public class Permiso
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;

    public override string ToString() => $"[{Modulo}] {Descripcion}";
}
