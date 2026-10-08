namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Página de resultados con la información necesaria para paginar en el cliente.
/// </summary>
/// <param name="Items">Registros de la página solicitada.</param>
/// <param name="Page">Número de página, desde 1.</param>
/// <param name="PageSize">Cantidad de registros por página.</param>
/// <param name="TotalCount">Total de registros disponibles.</param>
public record PagedResultDto<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalCount)
{
    /// <summary>Total de páginas.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
