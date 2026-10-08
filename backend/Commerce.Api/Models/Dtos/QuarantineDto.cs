namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Registro en cuarentena con el motivo del rechazo.
/// </summary>
public record QuarantineDto(
    int Id,
    DateOnly ProcessDate,
    string? CommerceCode,
    string? CommerceName,
    string? DocumentType,
    string? DocumentNumber,
    string? City,
    string Reason,
    DateTime QuarantinedAt);
