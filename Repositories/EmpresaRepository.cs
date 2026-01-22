using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GestorComercialCredito.Web.Repositories
{

    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly string _connectionString;

        public EmpresaRepository(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
            
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                // Intentar leer directamente desde la configuración
                var directConnection = configuration["ConnectionStrings:DefaultConnection"];
                if (!string.IsNullOrWhiteSpace(directConnection))
                {
                    _connectionString = directConnection;
                }
                else
                {
                    throw new InvalidOperationException($"Connection string 'DefaultConnection' not found in configuration. Available keys: {string.Join(", ", configuration.GetSection("ConnectionStrings").GetChildren().Select(c => c.Key))}");
                }
            }
        }

        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Connection string is null or empty.");

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT EmpresaId, Nit, Nombre, Activa, FechaCreacion
                    FROM Empresa";

                // Usar mapeo directo con Dapper
                var result = await connection.QueryAsync<Empresa>(sql);
                return result?.ToList() ?? Enumerable.Empty<Empresa>();
            }
            catch (SqlException sqlEx)
            {
                var connectionStringPreview = _connectionString?.Length > 50 
                    ? _connectionString.Substring(0, 50) + "..." 
                    : _connectionString ?? "null";
                    
                throw new Exception($"Error de SQL al obtener empresas: {sqlEx.Message}. Número de error: {sqlEx.Number}. ConnectionString: {connectionStringPreview}", sqlEx);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var connectionStringPreview = "null";
                try
                {
                    if (_connectionString != null && _connectionString.Length > 0)
                    {
                        connectionStringPreview = _connectionString.Length > 50 
                            ? _connectionString.Substring(0, 50) + "..." 
                            : _connectionString;
                    }
                }
                catch { }
                
                var innerExceptionMsg = ex.InnerException?.Message ?? "N/A";
                var errorMessage = $"Error al obtener empresas: {ex.Message ?? "Error desconocido"}. Tipo: {ex.GetType().Name}. InnerException: {innerExceptionMsg}. ConnectionString: {connectionStringPreview}";
                
                throw new Exception(errorMessage, ex);
            }
        }


        public async Task<Empresa?> GetByIdAsync(int empresaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT EmpresaId, Nit, Nombre, Activa, FechaCreacion
                FROM Empresa
                WHERE EmpresaId = @EmpresaId";

            return await connection.QueryFirstOrDefaultAsync<Empresa>(
                sql, new { EmpresaId = empresaId });
        }

        public async Task<Empresa?> GetByNitAsync(string nit)
        {
            if (string.IsNullOrWhiteSpace(nit)) return null;
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = @"
                SELECT EmpresaId, Nit, Nombre, Activa, FechaCreacion
                FROM Empresa
                WHERE Nit = @Nit";
            return await connection.QueryFirstOrDefaultAsync<Empresa>(sql, new { Nit = nit.Trim() });
        }


        public async Task<int> CreateAsync(Empresa empresa)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO Empresa (Nit, Nombre, Activa)
                VALUES (@Nit, @Nombre, @Activa);

                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, empresa);
        }


        public async Task<bool> UpdateAsync(Empresa empresa)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE Empresa
                SET 
                    Nit = @Nit,
                    Nombre = @Nombre,
                    Activa = @Activa
                WHERE EmpresaId = @EmpresaId";

            var rowsAffected = await connection.ExecuteAsync(sql, empresa);
            return rowsAffected > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM Empresa WHERE EmpresaId = @EmpresaId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { EmpresaId = id });
            return rowsAffected > 0;
        }

        public async Task<bool> InactivarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "UPDATE Empresa SET Activa = 0 WHERE EmpresaId = @EmpresaId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { EmpresaId = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ActivarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "UPDATE Empresa SET Activa = 1 WHERE EmpresaId = @EmpresaId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { EmpresaId = id });
            return rowsAffected > 0;
        }
    }
}
