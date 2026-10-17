namespace App.BLL.Entidades;

public class Plan
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoDuracion { get; set; } = "Mensual"; // Mensual / Trimestral / Anual
    public decimal Precio { get; set; }
    public bool IncluyeClases { get; set; }
    public bool Activo { get; set; } = true;

    /// <summary>Calcula la fecha de vencimiento de una inscripcion segun el tipo de plan.</summary>
    public DateTime CalcularVencimiento(DateTime fechaInicio) => TipoDuracion switch
    {
        "Trimestral" => fechaInicio.AddMonths(3),
        "Anual" => fechaInicio.AddYears(1),
        _ => fechaInicio.AddMonths(1)
    };

    public override string ToString() => Nombre;
}
