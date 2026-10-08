namespace Commerce.Api.Models.Entities;

/// <summary>
/// Resultado de <c>sp_process_commerce</c>.
/// </summary>
public class ProcessSummary
{
    /// <summary>Registros de la fecha que fueron revisados.</summary>
    public int ProcessedCount { get; set; }

    /// <summary>Registros enviados a cuarentena.</summary>
    public int QuarantinedCount { get; set; }
}
