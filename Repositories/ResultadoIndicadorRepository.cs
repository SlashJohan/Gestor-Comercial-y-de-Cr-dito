using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Repositories
{
    public class ResultadoIndicadorRepository : IResultadoIndicadorRepository
    {
        private readonly string _connectionString;

        public ResultadoIndicadorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<ResultadoIndicador>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT ri.ResultadoId, ri.EmpresaId, ri.PeriodoId, ri.IndicadorId, ri.Valor, ri.FechaCalculo,
                       e.EmpresaId, e.Nit, e.Nombre, e.Activa, e.FechaCreacion,
                       p.PeriodoId, p.Anio,
                       i.IndicadorId, i.Nombre, i.Descripcion, i.Activo
                FROM ResultadoIndicador ri
                LEFT JOIN Empresa e ON ri.EmpresaId = e.EmpresaId
                LEFT JOIN Periodo p ON ri.PeriodoId = p.PeriodoId
                LEFT JOIN Indicador i ON ri.IndicadorId = i.IndicadorId
                ORDER BY ri.FechaCalculo DESC";

            return await connection.QueryAsync<ResultadoIndicador, Empresa, Periodo, Indicador, ResultadoIndicador>(
                sql,
                (resultado, empresa, periodo, indicador) =>
                {
                    resultado.Empresa = empresa;
                    resultado.Periodo = periodo;
                    resultado.Indicador = indicador;
                    return resultado;
                },
                splitOn: "EmpresaId,PeriodoId,IndicadorId");
        }

        public async Task<IEnumerable<ResultadoIndicador>> GetByFiltrosAsync(int? empresaId, int? indicadorId, int? periodoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT ri.ResultadoId, ri.EmpresaId, ri.PeriodoId, ri.IndicadorId, ri.Valor, ri.FechaCalculo,
                       e.EmpresaId, e.Nit, e.Nombre, e.Activa, e.FechaCreacion,
                       p.PeriodoId, p.Anio,
                       i.IndicadorId, i.Nombre, i.Descripcion, i.Activo
                FROM ResultadoIndicador ri
                LEFT JOIN Empresa e ON ri.EmpresaId = e.EmpresaId
                LEFT JOIN Periodo p ON ri.PeriodoId = p.PeriodoId
                LEFT JOIN Indicador i ON ri.IndicadorId = i.IndicadorId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (empresaId.HasValue)
            {
                sql += " AND ri.EmpresaId = @EmpresaId";
                parameters.Add("EmpresaId", empresaId.Value);
            }

            if (indicadorId.HasValue)
            {
                sql += " AND ri.IndicadorId = @IndicadorId";
                parameters.Add("IndicadorId", indicadorId.Value);
            }

            if (periodoId.HasValue)
            {
                sql += " AND ri.PeriodoId = @PeriodoId";
                parameters.Add("PeriodoId", periodoId.Value);
            }

            sql += " ORDER BY ri.FechaCalculo DESC";

            return await connection.QueryAsync<ResultadoIndicador, Empresa, Periodo, Indicador, ResultadoIndicador>(
                sql,
                (resultado, empresa, periodo, indicador) =>
                {
                    resultado.Empresa = empresa;
                    resultado.Periodo = periodo;
                    resultado.Indicador = indicador;
                    return resultado;
                },
                parameters,
                splitOn: "EmpresaId,PeriodoId,IndicadorId");
        }

        public async Task<ResultadoIndicador?> GetByIdAsync(int resultadoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT ri.ResultadoId, ri.EmpresaId, ri.PeriodoId, ri.IndicadorId, ri.Valor, ri.FechaCalculo,
                       e.EmpresaId, e.Nit, e.Nombre, e.Activa, e.FechaCreacion,
                       p.PeriodoId, p.Anio,
                       i.IndicadorId, i.Nombre, i.Descripcion, i.Activo
                FROM ResultadoIndicador ri
                LEFT JOIN Empresa e ON ri.EmpresaId = e.EmpresaId
                LEFT JOIN Periodo p ON ri.PeriodoId = p.PeriodoId
                LEFT JOIN Indicador i ON ri.IndicadorId = i.IndicadorId
                WHERE ri.ResultadoId = @ResultadoId";

            var result = await connection.QueryAsync<ResultadoIndicador, Empresa, Periodo, Indicador, ResultadoIndicador>(
                sql,
                (resultado, empresa, periodo, indicador) =>
                {
                    resultado.Empresa = empresa;
                    resultado.Periodo = periodo;
                    resultado.Indicador = indicador;
                    return resultado;
                },
                new { ResultadoId = resultadoId },
                splitOn: "EmpresaId,PeriodoId,IndicadorId");

            return result.FirstOrDefault();
        }

        public async Task<int> CreateAsync(ResultadoIndicador resultadoIndicador)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO ResultadoIndicador (EmpresaId, PeriodoId, IndicadorId, Valor)
                VALUES (@EmpresaId, @PeriodoId, @IndicadorId, @Valor);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, resultadoIndicador);
        }

        public async Task<bool> UpdateAsync(ResultadoIndicador resultadoIndicador)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE ResultadoIndicador
                SET 
                    EmpresaId = @EmpresaId,
                    PeriodoId = @PeriodoId,
                    IndicadorId = @IndicadorId,
                    Valor = @Valor
                WHERE ResultadoId = @ResultadoId";

            var rowsAffected = await connection.ExecuteAsync(sql, resultadoIndicador);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM ResultadoIndicador WHERE ResultadoId = @ResultadoId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { ResultadoId = id });
            return rowsAffected > 0;
        }

        public async Task DeleteByEmpresaAndPeriodoAsync(int empresaId, int periodoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM ResultadoIndicador WHERE EmpresaId = @EmpresaId AND PeriodoId = @PeriodoId";
            await connection.ExecuteAsync(sql, new { EmpresaId = empresaId, PeriodoId = periodoId });
        }
    }
}
