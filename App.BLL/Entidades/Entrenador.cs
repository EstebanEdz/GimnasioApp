namespace App.BLL.Entidades;

public class Entrenador : Persona
{
    public string Especialidad { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }

    /// <summary>Horas mensuales estimadas de trabajo, usadas para el calculo de pago.</summary>
    public int HorasMensualesEstimadas { get; set; } = 160;

    public Entrenador() { }

    public Entrenador(string nombre, string documento, string telefono, string especialidad, decimal tarifaHora)
        : base(nombre, documento, telefono)
    {
        Especialidad = especialidad;
        TarifaHora = tarifaHora;
    }

    /// <summary>Polimorfismo: el "costo" de un entrenador es su pago mensual estimado.</summary>
    public override decimal CalcularCosto() => TarifaHora * HorasMensualesEstimadas;
}
