namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Parámetros de paginación recibidos por query string.
/// </summary>
public class PageRequestDto
{
    /// <summary>Número de página, desde 1.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Cantidad de registros por página (1 a 100).</summary>
    public int PageSize { get; set; } = 10;
}
