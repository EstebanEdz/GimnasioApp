namespace App.BLL.Entidades;

public static class EstadosMembresia
{
    public const string Vigente = "Vigente";
    public const string Vencida = "Vencida";
    public const string Congelada = "Congelada";
    public const string Cancelada = "Cancelada";

    public static readonly string[] Todos = { Vigente, Vencida, Congelada, Cancelada };
}

/// <summary>Vista de una inscripcion como "membresia": con su estado real, dias para vencer y saldo.</summary>
public class Membresia
{
    public int InscripcionId { get; set; }
    public int MiembroId { get; set; }
    public string Miembro { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string Plan { get; set; } = string.Empty;
    public string Entrenador { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int DiasParaVencer { get; set; }
    public decimal Total { get; set; }
    public decimal Pagado { get; set; }
    public decimal Saldo { get; set; }

    public bool PorVencer(int dias) => Estado == EstadosMembresia.Vigente && DiasParaVencer >= 0 && DiasParaVencer <= dias;

    public override string ToString() => $"#{InscripcionId} - {Miembro} ({Plan}) - saldo ${Saldo:N0}";
}

public class ResumenAlertas
{
    public int PorVencer { get; set; }
    public int Vencidas { get; set; }
    public int ConSaldo { get; set; }
}
