namespace Commerce.Api.Exceptions;

/// <summary>
/// Se lanza cuando los datos enviados por el cliente no son válidos.
/// El controlador la traduce a un 400 Bad Request.
/// </summary>
public class InvalidRequestException(string message) : Exception(message);
