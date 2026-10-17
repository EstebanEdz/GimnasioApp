using Microsoft.Data.SqlClient;

namespace App.DAL;

public class MembresiaRow
{
    public int InscripcionId { get; set; }
    public int MiembroId { get; set; }
    public string Miembro { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string PlanNombre { get; set; } = string.Empty;
    public string Entrenador { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string EstadoReal { get; set; } = string.Empty;
    public int DiasParaVencer { get; set; }
    public decimal Total { get; set; }
    public decimal Pagado { get; set; }
    public decimal Saldo { get; set; }
}

public class EstadoCantidadRow
{
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class MembresiaDAL
{
    private const string Select = @"
        SELECT InscripcionId, MiembroId, Miembro, Documento, PlanId, PlanNombre, Entrenador,
               FechaInscripcion, FechaVencimiento, EstadoReal, DiasParaVencer, Total, Pagado, Saldo
        FROM dbo.vw_Membresia";

    private static MembresiaRow Mapear(SqlDataReader r) => new MembresiaRow
    {
        InscripcionId = r.GetInt32(0),
        MiembroId = r.GetInt32(1),
        Miembro = r.GetString(2),
        Documento = r.GetString(3),
        PlanId = r.GetInt32(4),
        PlanNombre = r.GetString(5),
        Entrenador = r.GetString(6),
        FechaInscripcion = r.GetDateTime(7),
        FechaVencimiento = r.GetDateTime(8),
        EstadoReal = r.GetString(9),
        DiasParaVencer = r.GetInt32(10),
        Total = r.GetDecimal(11),
        Pagado = r.GetDecimal(12),
        Saldo = r.GetDecimal(13)
    };

    /// <summary>
    /// Lista membresias con filtros opcionales. estado: Vigente/Vencida/Congelada/Cancelada (null = todas).
    /// porVencerEnDias: solo Vigentes que vencen dentro de N dias. soloConSaldo: solo las que deben plata.
    /// </summary>
    public List<MembresiaRow> Listar(string? estado, string? texto, int? porVencerEnDias, bool soloConSaldo)
    {
        return DbHelper.Consultar(Select + @"
            WHERE (@estado IS NULL OR EstadoReal = @estado)
              AND (@texto IS NULL OR Miembro LIKE '%' + @texto + '%' OR Documento LIKE '%' + @texto + '%')
              AND (@dias IS NULL OR (EstadoReal = 'Vigente' AND DiasParaVencer BETWEEN 0 AND @dias))
              AND (@saldo = 0 OR (Saldo > 0 AND EstadoReal <> 'Cancelada'))
            ORDER BY FechaVencimiento, InscripcionId",
            Mapear,
            cmd =>
            {
                cmd.Parameters.AddWithValue("@estado", DbHelper.ONulo(string.IsNullOrWhiteSpace(estado) ? null : estado));
                cmd.Parameters.AddWithValue("@texto", DbHelper.ONulo(string.IsNullOrWhiteSpace(texto) ? null : texto.Trim()));
                cmd.Parameters.AddWithValue("@dias", DbHelper.ONulo(porVencerEnDias));
                cmd.Parameters.AddWithValue("@saldo", soloConSaldo ? 1 : 0);
            });
    }

    public MembresiaRow? ObtenerPorId(int inscripcionId) => DbHelper.Consultar(
        Select + " WHERE InscripcionId = @id", Mapear,
        cmd => cmd.Parameters.AddWithValue("@id", inscripcionId)).FirstOrDefault();

    public List<EstadoCantidadRow> ContarPorEstado() => DbHelper.Consultar(@"
        SELECT EstadoReal, COUNT(*) AS Cantidad
        FROM dbo.vw_Membresia
        GROUP BY EstadoReal
        ORDER BY Cantidad DESC",
        r => new EstadoCantidadRow { Estado = r.GetString(0), Cantidad = r.GetInt32(1) });

    /// <summary>Pasa a 'Vencida' las Vigentes cuya fecha ya paso. Devuelve cuantas cambio.</summary>
    public int ActualizarVencidas() => DbHelper.Ejecutar(@"
        UPDATE dbo.Inscripciones SET Estado = 'Vencida'
        WHERE Estado = 'Vigente' AND FechaVencimiento < CAST(GETDATE() AS DATE)");

    /// <summary>Congela una membresia Vigente (guarda desde cuando). Devuelve false si no estaba Vigente.</summary>
    public bool Congelar(int id) => DbHelper.Ejecutar(@"
        UPDATE dbo.Inscripciones
        SET Estado = 'Congelada', FechaCongelacion = CAST(GETDATE() AS DATE)
        WHERE Id = @id AND Estado = 'Vigente' AND FechaVencimiento >= CAST(GETDATE() AS DATE)",
        cmd => cmd.Parameters.AddWithValue("@id", id)) > 0;

    /// <summary>Reactiva una membresia Congelada y le suma al vencimiento los dias que estuvo congelada.</summary>
    public bool Descongelar(int id) => DbHelper.Ejecutar(@"
        UPDATE dbo.Inscripciones
        SET FechaVencimiento = DATEADD(DAY, DATEDIFF(DAY, FechaCongelacion, CAST(GETDATE() AS DATE)), FechaVencimiento),
            Estado = 'Vigente',
            FechaCongelacion = NULL
        WHERE Id = @id AND Estado = 'Congelada' AND FechaCongelacion IS NOT NULL",
        cmd => cmd.Parameters.AddWithValue("@id", id)) > 0;

    public bool Cancelar(int id) => DbHelper.Ejecutar(@"
        UPDATE dbo.Inscripciones SET Estado = 'Cancelada'
        WHERE Id = @id AND Estado IN ('Vigente', 'Vencida', 'Congelada')",
        cmd => cmd.Parameters.AddWithValue("@id", id)) > 0;
}
