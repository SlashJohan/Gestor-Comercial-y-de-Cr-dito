namespace GestorComercialCredito.Web.Services;

/// <summary>
/// Servicio para ejecutar procedimientos almacenados
/// </summary>
public interface IStoredProcedureService
{
    Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null);
    Task<T?> ExecuteStoredProcedureSingleAsync<T>(string procedureName, object? parameters = null);
    Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, object? parameters = null);
}
