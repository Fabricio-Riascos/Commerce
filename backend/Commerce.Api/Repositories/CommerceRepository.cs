using System.Data;
using Dapper;
using Commerce.Api.Models.Entities;
using Microsoft.Data.SqlClient;

namespace Commerce.Api.Repositories;

/// <summary>
/// Implementación de <see cref="ICommerceRepository"/> con Dapper sobre SQL Server.
/// </summary>
public class CommerceRepository(IConfiguration configuration) : ICommerceRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DbCommerce")
        ?? throw new InvalidOperationException("Connection string 'DbCommerce' is not configured.");

    /// <inheritdoc />
    public async Task<int> CreateAsync(IEnumerable<Models.Entities.Commerce> records)
    {
        var table = BuildCommerceTable(records);

        await using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_create_commerce",
            new { rows = table.AsTableValuedParameter("dbo.CommerceTableType") },
            commandType: CommandType.StoredProcedure);
    }

    /// <inheritdoc />
    public async Task<ProcessSummary> ProcessAsync(DateOnly processDate)
    {
        var parameters = new DynamicParameters();
        parameters.Add("processdate", processDate.ToDateTime(TimeOnly.MinValue), DbType.Date);

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleAsync<ProcessSummary>(
            "dbo.sp_process_commerce",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Models.Entities.Commerce> Items, int TotalCount)> GetCommerceAsync(
        DateOnly? processDate, int page, int pageSize)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM dbo.commerce
            WHERE @processdate IS NULL OR pc_processdate = @processdate;

            SELECT id             AS Id,
                   pc_processdate AS ProcessDate,
                   pc_codcomercio AS CommerceCode,
                   pc_nomcomred   AS CommerceName,
                   pc_tipodoc     AS DocumentType,
                   pc_numdoc      AS DocumentNumber,
                   pc_ciudad      AS City
            FROM dbo.commerce
            WHERE @processdate IS NULL OR pc_processdate = @processdate
            ORDER BY pc_processdate DESC, id
            OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY;
            """;

        var parameters = BuildPagingParameters(page, pageSize);
        parameters.Add("processdate", processDate?.ToDateTime(TimeOnly.MinValue), DbType.Date);

        await using var connection = new SqlConnection(_connectionString);
        await using var result = await connection.QueryMultipleAsync(sql, parameters);
        var total = await result.ReadSingleAsync<int>();
        var items = await result.ReadAsync<Models.Entities.Commerce>();
        return (items, total);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<CommerceQuarantine> Items, int TotalCount)> GetQuarantineAsync(
        int page, int pageSize)
    {
        const string sql = """
            SELECT COUNT(*) FROM dbo.commerce_quarantine;

            SELECT id               AS Id,
                   pc_processdate   AS ProcessDate,
                   pc_codcomercio   AS CommerceCode,
                   pc_nomcomred     AS CommerceName,
                   pc_tipodoc       AS DocumentType,
                   pc_numdoc        AS DocumentNumber,
                   pc_ciudad        AS City,
                   motivo           AS Reason,
                   fecha_cuarentena AS QuarantinedAt
            FROM dbo.commerce_quarantine
            ORDER BY fecha_cuarentena DESC, id DESC
            OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await using var result = await connection.QueryMultipleAsync(sql, BuildPagingParameters(page, pageSize));
        var total = await result.ReadSingleAsync<int>();
        var items = await result.ReadAsync<CommerceQuarantine>();
        return (items, total);
    }

    /// <summary>
    /// Parámetros de paginación para OFFSET / FETCH.
    /// </summary>
    private static DynamicParameters BuildPagingParameters(int page, int pageSize)
    {
        var parameters = new DynamicParameters();
        parameters.Add("offset", (page - 1) * pageSize, DbType.Int32);
        parameters.Add("pagesize", pageSize, DbType.Int32);
        return parameters;
    }

    /// <summary>
    /// Arma un DataTable con la misma estructura que el tipo <c>dbo.CommerceTableType</c>.
    /// </summary>
    private static DataTable BuildCommerceTable(IEnumerable<Models.Entities.Commerce> records)
    {
        var table = new DataTable();
        table.Columns.Add("pc_processdate", typeof(DateTime));
        table.Columns.Add("pc_codcomercio", typeof(string));
        table.Columns.Add("pc_nomcomred", typeof(string));
        table.Columns.Add("pc_tipodoc", typeof(string));
        table.Columns.Add("pc_numdoc", typeof(string));
        table.Columns.Add("pc_ciudad", typeof(string));

        foreach (var r in records)
        {
            table.Rows.Add(
                r.ProcessDate,
                (object?)r.CommerceCode ?? DBNull.Value,
                (object?)r.CommerceName ?? DBNull.Value,
                (object?)r.DocumentType ?? DBNull.Value,
                (object?)r.DocumentNumber ?? DBNull.Value,
                (object?)r.City ?? DBNull.Value);
        }

        return table;
    }
}
