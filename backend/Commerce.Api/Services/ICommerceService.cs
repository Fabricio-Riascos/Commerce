using Commerce.Api.Models.Dtos;

namespace Commerce.Api.Services;

/// <summary>
/// Lógica de negocio para la carga y el procesamiento de comercios.
/// </summary>
public interface ICommerceService
{
    /// <summary>
    /// Valida el archivo CSV, lo interpreta y guarda sus registros.
    /// </summary>
    /// <param name="file">Archivo recibido en la petición.</param>
    /// <returns>Nombre del archivo y cantidad de registros insertados.</returns>
    /// <exception cref="Exceptions.InvalidFileException">Si el archivo está vacío o no cumple el formato.</exception>
    Task<UploadResultDto> UploadAsync(IFormFile? file);

    /// <summary>
    /// Valida los registros de la fecha indicada y mueve los inválidos a cuarentena.
    /// </summary>
    /// <param name="request">Petición con la fecha de proceso.</param>
    /// <returns>Fecha procesada y cantidad de registros enviados a cuarentena.</returns>
    /// <exception cref="Exceptions.InvalidRequestException">Si no se envía la fecha.</exception>
    Task<ProcessResultDto> ProcessAsync(ProcessRequestDto request);

    /// <summary>
    /// Lista los registros en cuarentena con su motivo.
    /// </summary>
    /// <returns>Registros en cuarentena.</returns>
    Task<IEnumerable<QuarantineDto>> GetQuarantineAsync();
}
