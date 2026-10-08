namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Resultado del procesamiento de una fecha.
/// </summary>
/// <param name="ProcessDate">Fecha procesada.</param>
/// <param name="QuarantinedCount">Cantidad de registros enviados a cuarentena.</param>
public record ProcessResultDto(DateOnly ProcessDate, int QuarantinedCount);
