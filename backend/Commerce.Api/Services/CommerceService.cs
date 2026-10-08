using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Commerce.Api.Exceptions;
using Commerce.Api.Models.Dtos;
using Commerce.Api.Repositories;

namespace Commerce.Api.Services;

/// <summary>
/// Implementación de <see cref="ICommerceService"/>.
/// </summary>
public partial class CommerceService(ICommerceRepository repository) : ICommerceService
{
    private const char Separator = ';';
    private const int ExpectedColumns = 6;
    private const string RowDateFormat = "yyyy-MM-dd";
    private const int MaxPageSize = 100;

    [GeneratedRegex(@"^commerce_(\d{8})\.csv$", RegexOptions.IgnoreCase)]
    private static partial Regex FileNameRegex();

    /// <inheritdoc />
    public async Task<UploadResultDto> UploadAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            throw new InvalidFileException("El archivo está vacío o no fue enviado.");

        ValidateFileName(file.FileName);

        var records = await ParseCsvAsync(file);
        if (records.Count == 0)
            throw new InvalidFileException("El archivo no contiene registros.");

        var inserted = await repository.CreateAsync(records);
        return new UploadResultDto(file.FileName, inserted);
    }

    /// <inheritdoc />
    public async Task<ProcessResultDto> ProcessAsync(ProcessRequestDto request)
    {
        if (request.ProcessDate is not { } processDate)
            throw new InvalidRequestException("La fecha de proceso es obligatoria.");

        var summary = await repository.ProcessAsync(processDate);
        return new ProcessResultDto(processDate, summary.ProcessedCount, summary.QuarantinedCount);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<CommerceDto>> GetCommerceAsync(DateOnly? processDate, PageRequestDto paging)
    {
        ValidatePaging(paging);

        var (records, total) = await repository.GetCommerceAsync(processDate, paging.Page, paging.PageSize);
        var items = records.Select(r => new CommerceDto(
            r.Id,
            DateOnly.FromDateTime(r.ProcessDate),
            r.CommerceCode,
            r.CommerceName,
            r.DocumentType,
            r.DocumentNumber,
            r.City));

        return new PagedResultDto<CommerceDto>(items, paging.Page, paging.PageSize, total);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<QuarantineDto>> GetQuarantineAsync(PageRequestDto paging)
    {
        ValidatePaging(paging);

        var (records, total) = await repository.GetQuarantineAsync(paging.Page, paging.PageSize);
        var items = records.Select(r => new QuarantineDto(
            r.Id,
            DateOnly.FromDateTime(r.ProcessDate),
            r.CommerceCode,
            r.CommerceName,
            r.DocumentType,
            r.DocumentNumber,
            r.City,
            r.Reason,
            r.QuarantinedAt));

        return new PagedResultDto<QuarantineDto>(items, paging.Page, paging.PageSize, total);
    }

    /// <summary>
    /// Verifica que la página empiece en 1 y que el tamaño esté entre 1 y <see cref="MaxPageSize"/>.
    /// </summary>
    private static void ValidatePaging(PageRequestDto paging)
    {
        if (paging.Page < 1)
            throw new InvalidRequestException("El número de página debe ser mayor o igual a 1.");

        if (paging.PageSize is < 1 or > MaxPageSize)
            throw new InvalidRequestException($"El tamaño de página debe estar entre 1 y {MaxPageSize}.");
    }

    /// <summary>
    /// Verifica que el nombre cumpla <c>commerce_DDMMYYYY.csv</c> y que la fecha exista.
    /// </summary>
    private static void ValidateFileName(string fileName)
    {
        var match = FileNameRegex().Match(fileName);
        var isValidDate = match.Success && DateTime.TryParseExact(
            match.Groups[1].Value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

        if (!isValidDate)
            throw new InvalidFileException("El nombre del archivo debe tener el formato commerce_DDMMYYYY.csv.");
    }

    /// <summary>
    /// Lee el CSV (separado por ';', con encabezado) y lo convierte en entidades.
    /// Los campos vacíos se guardan como null para que el SP de proceso los detecte.
    /// </summary>
    private static async Task<List<Models.Entities.Commerce>> ParseCsvAsync(IFormFile file)
    {
        var records = new List<Models.Entities.Commerce>();

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);

        // Primera línea: encabezado.
        await reader.ReadLineAsync();

        var lineNumber = 1;
        while (await reader.ReadLineAsync() is { } line)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(Separator);
            if (columns.Length != ExpectedColumns)
                throw new InvalidFileException(
                    $"Línea {lineNumber}: se esperaban {ExpectedColumns} columnas y se encontraron {columns.Length}.");

            if (!DateTime.TryParseExact(columns[0].Trim(), RowDateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var processDate))
                throw new InvalidFileException(
                    $"Línea {lineNumber}: pc_processdate debe tener el formato {RowDateFormat}.");

            records.Add(new Models.Entities.Commerce
            {
                ProcessDate = processDate,
                CommerceCode = EmptyToNull(columns[1]),
                CommerceName = EmptyToNull(columns[2]),
                DocumentType = EmptyToNull(columns[3]),
                DocumentNumber = EmptyToNull(columns[4]),
                City = EmptyToNull(columns[5])
            });
        }

        return records;
    }

    private static string? EmptyToNull(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
