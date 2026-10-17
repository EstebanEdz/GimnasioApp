namespace App.BLL.Entidades;

public class Inscripcion
{
    public int Id { get; set; }
    public int MiembroId { get; set; }
    public string MiembroNombre { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string PlanNombre { get; set; } = string.Empty;
    public int? EntrenadorId { get; set; }
    public string EntrenadorNombre { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; } = DateTime.Today;
    public DateTime FechaVencimiento { get; set; }
    public string Estado { get; set; } = "Vigente";
    public decimal Total { get; set; }

    public List<DetalleInscripcion> Detalle { get; set; } = new();

    public override string ToString() => $"#{Id} - {MiembroNombre} ({PlanNombre})";
}
