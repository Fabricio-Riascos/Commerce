namespace Commerce.Api.Exceptions;

/// <summary>
/// Se lanza cuando el archivo recibido no cumple el formato esperado.
/// </summary>
public class InvalidFileException(string message) : InvalidRequestException(message);
