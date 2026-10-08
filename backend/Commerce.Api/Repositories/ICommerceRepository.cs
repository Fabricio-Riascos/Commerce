using Commerce.Api.Models.Entities;

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

    /// <summary>
    /// Ejecuta <c>sp_process_commerce</c> para la fecha indicada.
    /// </summary>
    /// <param name="processDate">Fecha de proceso.</param>
    /// <returns>Cantidad de registros enviados a cuarentena.</returns>
    Task<int> ProcessAsync(DateOnly processDate);

    /// <summary>
    /// Obtiene todos los registros de <c>commerce_quarantine</c>.
    /// </summary>
    /// <returns>Registros en cuarentena, del más reciente al más antiguo.</returns>
    Task<IEnumerable<CommerceQuarantine>> GetQuarantineAsync();
}
