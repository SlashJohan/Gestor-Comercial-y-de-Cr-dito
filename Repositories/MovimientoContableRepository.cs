using Dapper;
using GestorComercialCredito.Web.Models;
using Microsoft.Data.SqlClient;

namespace GestorComercialCredito.Web.Repositories
{
    public class MovimientoContableRepository : IMovimientoContableRepository
    {
        private readonly string _connectionString;

        public MovimientoContableRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<MovimientoContable>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT mc.MovimientoId, mc.EmpresaId, mc.PeriodoId, mc.CuentaId, mc.Valor,
                       e.EmpresaId, e.Nit, e.Nombre, e.Activa, e.FechaCreacion,
                       p.PeriodoId, p.Anio,
                       c.CuentaId, c.Codigo, c.Nombre, c.TipoCuenta, c.Activa
                FROM MovimientoContable mc
                LEFT JOIN Empresa e ON mc.EmpresaId = e.EmpresaId
                LEFT JOIN Periodo p ON mc.PeriodoId = p.PeriodoId
                LEFT JOIN CuentaPUC c ON mc.CuentaId = c.CuentaId
                ORDER BY mc.MovimientoId DESC";

            return await connection.QueryAsync<MovimientoContable, Empresa, Periodo, CuentaPUC, MovimientoContable>(
                sql,
                (movimiento, empresa, periodo, cuenta) =>
                {
                    movimiento.Empresa = empresa;
                    movimiento.Periodo = periodo;
                    movimiento.Cuenta = cuenta;
                    return movimiento;
                },
                splitOn: "EmpresaId,PeriodoId,CuentaId");
        }

        public async Task<IEnumerable<MovimientoContable>> GetByEmpresaAndPeriodoAsync(int empresaId, int periodoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT mc.MovimientoId, mc.EmpresaId, mc.PeriodoId, mc.CuentaId, mc.Valor,
                       e.EmpresaId, e.Nit, e.Nombre, e.Activa, e.FechaCreacion,
                       p.PeriodoId, p.Anio,
                       c.CuentaId, c.Codigo, c.Nombre, c.TipoCuenta, c.Activa
                FROM MovimientoContable mc
                LEFT JOIN Empresa e ON mc.EmpresaId = e.EmpresaId
                LEFT JOIN Periodo p ON mc.PeriodoId = p.PeriodoId
                LEFT JOIN CuentaPUC c ON mc.CuentaId = c.CuentaId
                WHERE mc.EmpresaId = @EmpresaId AND mc.PeriodoId = @PeriodoId
                ORDER BY mc.MovimientoId";

            return await connection.QueryAsync<MovimientoContable, Empresa, Periodo, CuentaPUC, MovimientoContable>(
                sql,
                (movimiento, empresa, periodo, cuenta) =>
                {
                    movimiento.Empresa = empresa;
                    movimiento.Periodo = periodo;
                    movimiento.Cuenta = cuenta;
                    return movimiento;
                },
                new { EmpresaId = empresaId, PeriodoId = periodoId },
                splitOn: "EmpresaId,PeriodoId,CuentaId");
        }

        public async Task<MovimientoContable?> GetByIdAsync(int movimientoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT mc.MovimientoId, mc.EmpresaId, mc.PeriodoId, mc.CuentaId, mc.Valor,
                       e.EmpresaId, e.Nit, e.Nombre, e.Activa, e.FechaCreacion,
                       p.PeriodoId, p.Anio,
                       c.CuentaId, c.Codigo, c.Nombre, c.TipoCuenta, c.Activa
                FROM MovimientoContable mc
                LEFT JOIN Empresa e ON mc.EmpresaId = e.EmpresaId
                LEFT JOIN Periodo p ON mc.PeriodoId = p.PeriodoId
                LEFT JOIN CuentaPUC c ON mc.CuentaId = c.CuentaId
                WHERE mc.MovimientoId = @MovimientoId";

            var result = await connection.QueryAsync<MovimientoContable, Empresa, Periodo, CuentaPUC, MovimientoContable>(
                sql,
                (movimiento, empresa, periodo, cuenta) =>
                {
                    movimiento.Empresa = empresa;
                    movimiento.Periodo = periodo;
                    movimiento.Cuenta = cuenta;
                    return movimiento;
                },
                new { MovimientoId = movimientoId },
                splitOn: "EmpresaId,PeriodoId,CuentaId");

            return result.FirstOrDefault();
        }

        public async Task<int> CreateAsync(MovimientoContable movimiento)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO MovimientoContable (EmpresaId, PeriodoId, CuentaId, Valor)
                VALUES (@EmpresaId, @PeriodoId, @CuentaId, @Valor);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.QuerySingleAsync<int>(sql, movimiento);
        }

        public async Task CreateRangeAsync(IEnumerable<MovimientoContable> movimientos)
        {
            var list = movimientos.ToList();
            if (list.Count == 0) return;
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = @"
                INSERT INTO MovimientoContable (EmpresaId, PeriodoId, CuentaId, Valor)
                VALUES (@EmpresaId, @PeriodoId, @CuentaId, @Valor)";
            await connection.ExecuteAsync(sql, list);
        }

        public async Task<bool> UpdateAsync(MovimientoContable movimiento)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE MovimientoContable
                SET 
                    EmpresaId = @EmpresaId,
                    PeriodoId = @PeriodoId,
                    CuentaId = @CuentaId,
                    Valor = @Valor
                WHERE MovimientoId = @MovimientoId";

            var rowsAffected = await connection.ExecuteAsync(sql, movimiento);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            const string sql = "DELETE FROM MovimientoContable WHERE MovimientoId = @MovimientoId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { MovimientoId = id });
            return rowsAffected > 0;
        }
    }
}
