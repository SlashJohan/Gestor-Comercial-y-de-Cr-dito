using Dapper;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Services;

public class StoredProcedureService : IStoredProcedureService
{
    private readonly string _connectionString;

    public StoredProcedureService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<T?> ExecuteStoredProcedureSingleAsync<T>(string procedureName, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteAsync(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
    }
}
