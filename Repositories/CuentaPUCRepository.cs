using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Repositories
{
    public class CuentaPUCRepository : ICuentaPUCRepository
    {
        private readonly string _connectionString;

        public CuentaPUCRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<CuentaPUC>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT CuentaId, Codigo, Nombre, TipoCuenta, Activa
                FROM CuentaPUC
                ORDER BY Codigo";

            return await connection.QueryAsync<CuentaPUC>(sql);
        }

        public async Task<CuentaPUC?> GetByIdAsync(int cuentaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT CuentaId, Codigo, Nombre, TipoCuenta, Activa
                FROM CuentaPUC
                WHERE CuentaId = @CuentaId";

            return await connection.QueryFirstOrDefaultAsync<CuentaPUC>(
                sql, new { CuentaId = cuentaId });
        }

        public async Task<CuentaPUC?> GetByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = @"
                SELECT CuentaId, Codigo, Nombre, TipoCuenta, Activa
                FROM CuentaPUC
                WHERE Codigo = @Codigo";
            return await connection.QueryFirstOrDefaultAsync<CuentaPUC>(sql, new { Codigo = codigo.Trim() });
        }

        public async Task<int> CreateAsync(CuentaPUC cuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO CuentaPUC (Codigo, Nombre, TipoCuenta, Activa)
                VALUES (@Codigo, @Nombre, @TipoCuenta, @Activa);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, cuenta);
        }

        public async Task<bool> UpdateAsync(CuentaPUC cuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE CuentaPUC
                SET 
                    Codigo = @Codigo,
                    Nombre = @Nombre,
                    TipoCuenta = @TipoCuenta,
                    Activa = @Activa
                WHERE CuentaId = @CuentaId";

            var rowsAffected = await connection.ExecuteAsync(sql, cuenta);
            return rowsAffected > 0;
        }

        public async Task<bool> InactivarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "UPDATE CuentaPUC SET Activa = 0 WHERE CuentaId = @CuentaId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { CuentaId = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ActivarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "UPDATE CuentaPUC SET Activa = 1 WHERE CuentaId = @CuentaId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { CuentaId = id });
            return rowsAffected > 0;
        }
    }
}
