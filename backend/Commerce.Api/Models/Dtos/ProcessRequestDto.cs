namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Petición para procesar los registros de una fecha.
/// </summary>
/// <param name="ProcessDate">Fecha de proceso (yyyy-MM-dd) a validar.</param>
public record ProcessRequestDto(DateOnly? ProcessDate);
