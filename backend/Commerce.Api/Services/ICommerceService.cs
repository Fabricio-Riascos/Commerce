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
}
