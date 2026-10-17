namespace App.BLL.Entidades;

public static class MetodosPago
{
    public static readonly string[] Todos = { "Efectivo", "Tarjeta", "Transferencia", "Nequi", "Daviplata" };

    /// <summary>Metodos que normalmente traen un numero de comprobante.</summary>
    public static bool RequiereReferencia(string metodo) => metodo != "Efectivo";
}

public class Pago
{
    public int Id { get; set; }
    public int InscripcionId { get; set; }
    public string Miembro { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public DateTime FechaPago { get; set; } = DateTime.Now;
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public string Referencia { get; set; } = string.Empty;
    public string Estado { get; set; } = "Aplicado";
    public string Usuario { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public string MotivoAnulacion { get; set; } = string.Empty;
}

/// <summary>Resumen de cuenta de una inscripcion: cuanto vale, cuanto se ha pagado y cuanto falta.</summary>
public class SaldoInscripcion
{
    public int InscripcionId { get; set; }
    public decimal Total { get; set; }
    public decimal Pagado { get; set; }
    public decimal Saldo { get; set; }

    public string EstadoPago => Saldo <= 0 ? "Pagada" : Pagado > 0 ? "Parcial" : "Pendiente";
}
