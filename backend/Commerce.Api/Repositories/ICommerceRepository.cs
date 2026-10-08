namespace Commerce.Api.Repositories;

/// <summary>
/// Acceso a datos de comercios mediante stored procedures.
/// </summary>
public interface ICommerceRepository
{
    /// <summary>
    /// Inserta los registros en la tabla commerce usando <c>sp_create_commerce</c>.
    /// </summary>
    /// <param name="records">Registros leídos del archivo.</param>
    /// <returns>Cantidad de registros insertados.</returns>
    Task<int> CreateAsync(IEnumerable<Models.Entities.Commerce> records);
}
