namespace App.BLL.Entidades;

/// <summary>
/// Clase base para el requisito de herencia (Ses. 5). Encapsula los datos
/// comunes a Miembro y Entrenador y define un metodo abstracto que cada
/// subtipo calcula de forma distinta (polimorfismo).
/// </summary>
public abstract class Persona
{
    private int _id;
    private string _nombre = string.Empty;
    private string _documento = string.Empty;
    private string _telefono = string.Empty;

    public int Id { get => _id; set => _id = value; }
    public string Nombre { get => _nombre; set => _nombre = value; }
    public string Documento { get => _documento; set => _documento = value; }
    public string Telefono { get => _telefono; set => _telefono = value; }
    public bool Activo { get; set; } = true;

    protected Persona() { }

    protected Persona(string nombre, string documento, string telefono)
    {
        Nombre = nombre;
        Documento = documento;
        Telefono = telefono;
    }

    /// <summary>Cada subtipo define su propio calculo (cuota vs. pago por horas).</summary>
    public abstract decimal CalcularCosto();

    public override string ToString() => Nombre;
}
