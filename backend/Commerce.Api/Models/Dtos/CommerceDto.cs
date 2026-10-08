namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Registro almacenado en la tabla commerce.
/// </summary>
public record CommerceDto(
    int Id,
    DateOnly ProcessDate,
    string? CommerceCode,
    string? CommerceName,
    string? DocumentType,
    string? DocumentNumber,
    string? City);
