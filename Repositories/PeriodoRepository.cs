using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Repositories
{
    public class PeriodoRepository : IPeriodoRepository
    {
        private readonly string _connectionString;

        public PeriodoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<Periodo>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT PeriodoId, Anio
                FROM Periodo
                ORDER BY Anio DESC";

            return await connection.QueryAsync<Periodo>(sql);
        }

        public async Task<Periodo?> GetByIdAsync(int periodoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT PeriodoId, Anio
                FROM Periodo
                WHERE PeriodoId = @PeriodoId";

            return await connection.QueryFirstOrDefaultAsync<Periodo>(
                sql, new { PeriodoId = periodoId });
        }

        public async Task<Periodo?> GetByAnioAsync(int anio)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT PeriodoId, Anio
                FROM Periodo
                WHERE Anio = @Anio";

            return await connection.QueryFirstOrDefaultAsync<Periodo>(
                sql, new { Anio = anio });
        }

        public async Task<int> CreateAsync(Periodo periodo)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO Periodo (Anio)
                VALUES (@Anio);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, periodo);
        }

        public async Task<bool> UpdateAsync(Periodo periodo)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE Periodo
                SET Anio = @Anio
                WHERE PeriodoId = @PeriodoId";

            var rowsAffected = await connection.ExecuteAsync(sql, periodo);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM Periodo WHERE PeriodoId = @PeriodoId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { PeriodoId = id });
            return rowsAffected > 0;
        }
    }
}
