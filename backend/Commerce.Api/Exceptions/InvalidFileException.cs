namespace Commerce.Api.Exceptions;

/// <summary>
/// Se lanza cuando el archivo recibido no cumple el formato esperado.
/// El controlador la traduce a un 400 Bad Request.
/// </summary>
public class InvalidFileException(string message) : Exception(message);
