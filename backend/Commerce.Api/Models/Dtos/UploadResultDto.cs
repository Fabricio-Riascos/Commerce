namespace Commerce.Api.Models.Dtos;

/// <summary>
/// Resultado de la carga de un archivo CSV.
/// </summary>
/// <param name="FileName">Nombre del archivo cargado.</param>
/// <param name="InsertedCount">Cantidad de registros insertados en la tabla commerce.</param>
public record UploadResultDto(string FileName, int InsertedCount);
