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
    /// <returns>Cantidad de registros revisados y enviados a cuarentena.</returns>
    Task<ProcessSummary> ProcessAsync(DateOnly processDate);

    /// <summary>
    /// Obtiene una página de la tabla <c>commerce</c>.
    /// </summary>
    /// <param name="processDate">Filtro opcional por fecha de proceso.</param>
    /// <param name="page">Número de página, desde 1.</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
    /// <returns>Registros de la página y total de registros que cumplen el filtro.</returns>
    Task<(IEnumerable<Models.Entities.Commerce> Items, int TotalCount)> GetCommerceAsync(
        DateOnly? processDate, int page, int pageSize);

    /// <summary>
    /// Obtiene una página de <c>commerce_quarantine</c>, del más reciente al más antiguo.
    /// </summary>
    /// <param name="page">Número de página, desde 1.</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
    /// <returns>Registros de la página y total de registros en cuarentena.</returns>
    Task<(IEnumerable<CommerceQuarantine> Items, int TotalCount)> GetQuarantineAsync(int page, int pageSize);
}
