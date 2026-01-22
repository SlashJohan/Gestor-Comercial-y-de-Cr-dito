using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface ICuentaPUCRepository
{
    Task<IEnumerable<CuentaPUC>> GetAllAsync();
    Task<CuentaPUC?> GetByIdAsync(int id);
    Task<CuentaPUC?> GetByCodigoAsync(string codigo);
    Task<int> CreateAsync(CuentaPUC cuenta);
    Task<bool> UpdateAsync(CuentaPUC cuenta);
    Task<bool> InactivarAsync(int id);
    Task<bool> ActivarAsync(int id);
}
