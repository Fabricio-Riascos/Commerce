using Commerce.Api.Exceptions;
using Commerce.Api.Models.Dtos;
using Commerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

/// <summary>
/// Endpoints para cargar, procesar y consultar comercios.
/// </summary>
[ApiController]
[Route("api/commerce")]
public class CommerceController(ICommerceService service) : ControllerBase
{
    /// <summary>
    /// Recibe un archivo commerce_DDMMYYYY.csv y guarda sus registros.
    /// </summary>
    /// <param name="file">Archivo CSV enviado como multipart/form-data.</param>
    /// <response code="200">Archivo cargado correctamente.</response>
    /// <response code="400">Archivo vacío, nombre inválido o contenido con formato incorrecto.</response>
    [HttpPost("upload")]
    [ProducesResponseType<UploadResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        try
        {
            return Ok(await service.UploadAsync(file));
        }
        catch (InvalidFileException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
