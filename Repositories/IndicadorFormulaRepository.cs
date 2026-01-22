using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Repositories
{
    public class IndicadorFormulaRepository : IIndicadorFormulaRepository
    {
        private readonly string _connectionString;

        public IndicadorFormulaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<IndicadorFormula>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT f.FormulaId, f.IndicadorId, f.FormulaSQL,
                       i.IndicadorId, i.Nombre, i.Descripcion, i.Activo
                FROM IndicadorFormula f
                LEFT JOIN Indicador i ON f.IndicadorId = i.IndicadorId
                ORDER BY f.FormulaId";

            return await connection.QueryAsync<IndicadorFormula, Indicador, IndicadorFormula>(
                sql,
                (formula, indicador) =>
                {
                    formula.Indicador = indicador;
                    return formula;
                },
                splitOn: "IndicadorId");
        }

        public async Task<IndicadorFormula?> GetByIdAsync(int formulaId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT f.FormulaId, f.IndicadorId, f.FormulaSQL,
                       i.IndicadorId, i.Nombre, i.Descripcion, i.Activo
                FROM IndicadorFormula f
                LEFT JOIN Indicador i ON f.IndicadorId = i.IndicadorId
                WHERE f.FormulaId = @FormulaId";

            var result = await connection.QueryAsync<IndicadorFormula, Indicador, IndicadorFormula>(
                sql,
                (formula, indicador) =>
                {
                    formula.Indicador = indicador;
                    return formula;
                },
                new { FormulaId = formulaId },
                splitOn: "IndicadorId");

            return result.FirstOrDefault();
        }

        public async Task<IndicadorFormula?> GetByIndicadorIdAsync(int indicadorId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT f.FormulaId, f.IndicadorId, f.FormulaSQL,
                       i.IndicadorId, i.Nombre, i.Descripcion, i.Activo
                FROM IndicadorFormula f
                LEFT JOIN Indicador i ON f.IndicadorId = i.IndicadorId
                WHERE f.IndicadorId = @IndicadorId";

            var result = await connection.QueryAsync<IndicadorFormula, Indicador, IndicadorFormula>(
                sql,
                (formula, indicador) =>
                {
                    formula.Indicador = indicador;
                    return formula;
                },
                new { IndicadorId = indicadorId },
                splitOn: "IndicadorId");

            return result.FirstOrDefault();
        }

        public async Task<int> CreateAsync(IndicadorFormula formula)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO IndicadorFormula (IndicadorId, FormulaSQL)
                VALUES (@IndicadorId, @FormulaSQL);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, formula);
        }

        public async Task<bool> UpdateAsync(IndicadorFormula formula)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE IndicadorFormula
                SET 
                    IndicadorId = @IndicadorId,
                    FormulaSQL = @FormulaSQL
                WHERE FormulaId = @FormulaId";

            var rowsAffected = await connection.ExecuteAsync(sql, formula);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM IndicadorFormula WHERE FormulaId = @FormulaId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { FormulaId = id });
            return rowsAffected > 0;
        }
    }
}
