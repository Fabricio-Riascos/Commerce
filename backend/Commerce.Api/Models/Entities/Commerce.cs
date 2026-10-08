namespace Commerce.Api.Models.Entities;

/// <summary>
/// Registro de comercio tal como se guarda en la tabla <c>commerce</c>.
/// </summary>
public class Commerce
{
    public DateTime ProcessDate { get; set; }
    public string? CommerceCode { get; set; }
    public string? CommerceName { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public string? City { get; set; }
}
