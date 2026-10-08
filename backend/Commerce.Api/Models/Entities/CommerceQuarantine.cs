namespace Commerce.Api.Models.Entities;

/// <summary>
/// Registro rechazado por el proceso de validación, tabla <c>commerce_quarantine</c>.
/// </summary>
public class CommerceQuarantine : Commerce
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime QuarantinedAt { get; set; }
}
