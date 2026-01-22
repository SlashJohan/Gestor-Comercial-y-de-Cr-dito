using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface IMovimientoContableRepository
{
    Task<IEnumerable<MovimientoContable>> GetAllAsync();
    Task<IEnumerable<MovimientoContable>> GetByEmpresaAndPeriodoAsync(int empresaId, int periodoId);
    Task<MovimientoContable?> GetByIdAsync(int id);
    Task<int> CreateAsync(MovimientoContable movimiento);
    Task CreateRangeAsync(IEnumerable<MovimientoContable> movimientos);
    Task<bool> UpdateAsync(MovimientoContable movimiento);
    Task<bool> DeleteAsync(int id);
}
