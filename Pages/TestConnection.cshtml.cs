using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GestorComercialCredito.Web.Pages;

public class TestConnectionModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TestConnectionModel> _logger;

    public TestConnectionModel(IConfiguration configuration, ILogger<TestConnectionModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string? ConnectionString { get; set; }
    public string? TestResult { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> DatabaseInfo { get; set; } = new();

    public void OnGet()
    {
        try
        {
            // Asegurar que DatabaseInfo esté inicializado
            if (DatabaseInfo == null)
            {
                DatabaseInfo = new List<string>();
            }

            ConnectionString = _configuration?.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                ErrorMessage = "La cadena de conexión 'DefaultConnection' no se encontró en la configuración.";
                TestResult = "✗ Error de configuración";
                return;
            }

            // Intentar conectar
            TestConnection();
        }
        catch (Exception ex)
        {
            TestResult = "✗ Error de conexión";
            ErrorMessage = $"Error: {ex.Message}";
            if (ex.InnerException != null)
            {
                ErrorMessage += $" InnerException: {ex.InnerException.Message}";
            }
            _logger?.LogError(ex, "Error al probar la conexión");
            
            // Asegurar que DatabaseInfo esté inicializado incluso en caso de error
            if (DatabaseInfo == null)
            {
                DatabaseInfo = new List<string>();
            }
        }
    }

    private void TestConnection()
    {
        try
        {
            // Asegurar que DatabaseInfo esté inicializado
            if (DatabaseInfo == null)
            {
                DatabaseInfo = new List<string>();
            }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                TestResult = "✗ Error de conexión";
                ErrorMessage = "La cadena de conexión está vacía o es null.";
                return;
            }

            // Log de la cadena de conexión (sin mostrar credenciales completas)
            var safeConnectionString = ConnectionString?.Length > 50 
                ? ConnectionString.Substring(0, 50) + "..." 
                : ConnectionString ?? "null";
            
            DatabaseInfo.Add($"Cadena de conexión completa: {ConnectionString}");
            DatabaseInfo.Add($"Longitud de cadena: {ConnectionString?.Length ?? 0}");
            
            // Verificar que la cadena de conexión tenga los elementos necesarios
            if (ConnectionString != null)
            {
                if (!ConnectionString.Contains("Server="))
                {
                    DatabaseInfo.Add("⚠ ADVERTENCIA: La cadena no contiene 'Server='");
                }
                if (!ConnectionString.Contains("Database="))
                {
                    DatabaseInfo.Add("⚠ ADVERTENCIA: La cadena no contiene 'Database='");
                }
            }
            
            DatabaseInfo.Add($"Intentando conectar con: {safeConnectionString}");
            
            // Validar que ConnectionString no sea null antes de crear la conexión
            if (ConnectionString == null)
            {
                throw new InvalidOperationException("ConnectionString es null");
            }
            
            SqlConnection? connection = null;
            try
            {
                DatabaseInfo.Add($"Creando objeto SqlConnection...");
                
                // Usar directamente la cadena de conexión sin SqlConnectionStringBuilder
                // para evitar problemas con NullReferenceException
                connection = new SqlConnection(ConnectionString);
                
                if (connection == null)
                {
                    throw new InvalidOperationException("No se pudo crear el objeto SqlConnection");
                }
                
                DatabaseInfo.Add($"Conexión creada exitosamente.");
                DatabaseInfo.Add($"Intentando abrir conexión...");
                
                // Intentar abrir la conexión de forma síncrona (más estable)
                connection.Open();
                
                DatabaseInfo.Add($"Conexión abierta exitosamente!");
        
            TestResult = "✓ Conexión exitosa a la base de datos";
            
            // Obtener información de la base de datos
            var dbName = connection.Database;
            var serverName = connection.DataSource;
            
            DatabaseInfo.Add($"Servidor: {serverName}");
            DatabaseInfo.Add($"Base de datos: {dbName}");
            DatabaseInfo.Add($"Estado: {connection.State}");
            
            // Verificar si las tablas existen
            var tables = new[] { "Empresa", "CuentaPUC", "Indicador", "Periodo", "MovimientoContable", "ResultadoIndicador", "IndicadorFormula" };
            
            foreach (var table in tables)
            {
                try
                {
                    var checkTableSql = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = @TableName";
                    
                    var exists = connection.ExecuteScalar<int>(checkTableSql, new { TableName = table }) > 0;
                    DatabaseInfo.Add($"Tabla {table}: {(exists ? "✓ Existe" : "✗ No existe")}");
                }
                catch (Exception tableEx)
                {
                    DatabaseInfo.Add($"Tabla {table}: ✗ Error al verificar: {tableEx.Message}");
                }
            }

            // Probar una consulta simple a la tabla Empresa
            try
            {
                var testSql = "SELECT TOP 1 EmpresaId, Nit, Nombre, Activa, FechaCreacion FROM Empresa";
                var testResult = connection.QueryFirstOrDefault<dynamic>(testSql);
                if (testResult != null)
                {
                    DatabaseInfo.Add($"Prueba de consulta: ✓ La tabla Empresa tiene datos y es accesible");
                    DatabaseInfo.Add($"  - EmpresaId: {testResult.EmpresaId}");
                    DatabaseInfo.Add($"  - Nit: {testResult.Nit}");
                    DatabaseInfo.Add($"  - Nombre: {testResult.Nombre}");
                }
                else
                {
                    DatabaseInfo.Add($"Prueba de consulta: ⚠ La tabla Empresa existe pero está vacía");
                }
            }
            catch (Exception queryEx)
            {
                DatabaseInfo.Add($"Prueba de consulta: ✗ Error al consultar: {queryEx.Message}");
                _logger?.LogWarning(queryEx, "Error al consultar la tabla Empresa");
            }
            }
            finally
            {
                if (connection != null)
                {
                    try
                    {
                        if (connection.State != System.Data.ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                        connection.Dispose();
                    }
                    catch { }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            TestResult = "✗ Error de conexión SQL";
            ErrorMessage = $"Error SQL {sqlEx.Number}: {sqlEx.Message}";
            
            // Agregar información detallada del error
            if (DatabaseInfo == null)
            {
                DatabaseInfo = new List<string>();
            }
            
            DatabaseInfo.Add($"Error SQL Number: {sqlEx.Number}");
            DatabaseInfo.Add($"Error SQL Class: {sqlEx.Class}");
            DatabaseInfo.Add($"Error SQL State: {sqlEx.State}");
            DatabaseInfo.Add($"Error SQL Server: {sqlEx.Server}");
            DatabaseInfo.Add($"StackTrace: {(sqlEx.StackTrace != null ? sqlEx.StackTrace.Substring(0, Math.Min(200, sqlEx.StackTrace.Length)) : "N/A")}");
            
            if (sqlEx.Number == -1)
            {
                ErrorMessage += " (Posible problema: El servidor no está accesible o la base de datos no existe)";
            }
            else if (sqlEx.Number == 2)
            {
                ErrorMessage += " (Posible problema: Timeout - El servidor no responde)";
            }
            else if (sqlEx.Number == 53)
            {
                ErrorMessage += " (Posible problema: No se puede encontrar el servidor SQL Server)";
            }
            else if (sqlEx.Number == 18456)
            {
                ErrorMessage += " (Posible problema: Error de autenticación - Usuario o contraseña incorrectos)";
            }
            
            _logger?.LogError(sqlEx, "Error SQL al conectar");
        }
        catch (Exception ex)
        {
            TestResult = "✗ Error de conexión";
            ErrorMessage = $"Error: {ex.Message}. Tipo: {ex.GetType().Name}";
            if (ex.InnerException != null)
            {
                ErrorMessage += $". InnerException: {ex.InnerException.Message}";
            }
            
            // Asegurar que DatabaseInfo esté inicializado
            if (DatabaseInfo == null)
            {
                DatabaseInfo = new List<string>();
            }
            
            // Agregar información detallada del error
            DatabaseInfo.Add($"Tipo de excepción: {ex.GetType().FullName}");
            DatabaseInfo.Add($"Mensaje completo: {ex.Message}");
            if (ex.InnerException != null)
            {
                DatabaseInfo.Add($"InnerException Tipo: {ex.InnerException.GetType().Name}");
                DatabaseInfo.Add($"InnerException Mensaje: {ex.InnerException.Message}");
            }
            DatabaseInfo.Add($"StackTrace (primeros 300 chars): {(ex.StackTrace != null ? ex.StackTrace.Substring(0, Math.Min(300, ex.StackTrace.Length)) : "N/A")}");
            
            _logger?.LogError(ex, "Error general al conectar");
        }
    }
}
