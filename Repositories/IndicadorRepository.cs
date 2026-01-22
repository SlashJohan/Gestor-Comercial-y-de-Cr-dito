using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Repositories
{
    public class IndicadorRepository : IIndicadorRepository
    {
        private readonly string _connectionString;

        public IndicadorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<Indicador>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT IndicadorId, Nombre, Descripcion, Activo
                FROM Indicador
                ORDER BY Nombre";

            return await connection.QueryAsync<Indicador>(sql);
        }

        public async Task<Indicador?> GetByIdAsync(int indicadorId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT IndicadorId, Nombre, Descripcion, Activo
                FROM Indicador
                WHERE IndicadorId = @IndicadorId";

            return await connection.QueryFirstOrDefaultAsync<Indicador>(
                sql, new { IndicadorId = indicadorId });
        }

        public async Task<int> CreateAsync(Indicador indicador)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO Indicador (Nombre, Descripcion, Activo)
                VALUES (@Nombre, @Descripcion, @Activo);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, indicador);
        }

        public async Task<bool> UpdateAsync(Indicador indicador)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE Indicador
                SET 
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Activo = @Activo
                WHERE IndicadorId = @IndicadorId";

            var rowsAffected = await connection.ExecuteAsync(sql, indicador);
            return rowsAffected > 0;
        }

        public async Task<bool> InactivarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "UPDATE Indicador SET Activo = 0 WHERE IndicadorId = @IndicadorId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { IndicadorId = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ActivarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "UPDATE Indicador SET Activo = 1 WHERE IndicadorId = @IndicadorId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { IndicadorId = id });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM Indicador WHERE IndicadorId = @IndicadorId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { IndicadorId = id });
            return rowsAffected > 0;
        }
    }
}
