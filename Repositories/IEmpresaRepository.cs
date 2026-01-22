using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface IEmpresaRepository
{
    Task<IEnumerable<Empresa>> GetAllAsync();
    Task<Empresa?> GetByIdAsync(int id);
    Task<Empresa?> GetByNitAsync(string nit);
    Task<int> CreateAsync(Empresa empresa);
    Task<bool> UpdateAsync(Empresa empresa);
    Task<bool> DeleteAsync(int id);
    Task<bool> InactivarAsync(int id);
    Task<bool> ActivarAsync(int id);
}
