namespace App.BLL.Entidades;

public class Miembro : Persona
{
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; } = DateTime.Today;

    /// <summary>Precio del plan vigente del miembro; lo asigna el servicio al construir la lista.</summary>
    public decimal PrecioPlanVigente { get; set; }

    public Miembro() { }

    public Miembro(string nombre, string documento, string telefono, string correo, DateTime fechaIngreso)
        : base(nombre, documento, telefono)
    {
        Correo = correo;
        FechaIngreso = fechaIngreso;
    }

    /// <summary>Polimorfismo: el "costo" de un miembro es la cuota de su plan vigente.</summary>
    public override decimal CalcularCosto() => PrecioPlanVigente;
}
