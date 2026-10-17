namespace App.BLL.Entidades;

public class DetalleInscripcion
{
    public int Id { get; set; }
    public int InscripcionId { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public int Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
