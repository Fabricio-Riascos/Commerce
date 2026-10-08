using System.Data;
using Dapper;
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
