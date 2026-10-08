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
        catch (InvalidRequestException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Valida los registros de una fecha y envía los inválidos a cuarentena.
    /// </summary>
    /// <param name="request">Cuerpo JSON con la fecha de proceso, por ejemplo <c>{ "processDate": "2026-10-07" }</c>.</param>
    /// <response code="200">Proceso ejecutado; retorna la cantidad enviada a cuarentena.</response>
    /// <response code="400">Fecha ausente o con formato inválido.</response>
    [HttpPost("process")]
    [ProducesResponseType<ProcessResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Process([FromBody] ProcessRequestDto request)
    {
        try
        {
            return Ok(await service.ProcessAsync(request));
        }
        catch (InvalidRequestException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Lista los registros en cuarentena con su motivo.
    /// </summary>
    /// <response code="200">Lista de registros (puede estar vacía).</response>
    [HttpGet("quarantine")]
    [ProducesResponseType<IEnumerable<QuarantineDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuarantine() =>
        Ok(await service.GetQuarantineAsync());
}
