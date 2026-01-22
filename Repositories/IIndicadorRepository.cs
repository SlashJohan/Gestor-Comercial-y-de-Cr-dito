using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface IIndicadorRepository
{
    Task<IEnumerable<Indicador>> GetAllAsync();
    Task<Indicador?> GetByIdAsync(int id);
    Task<int> CreateAsync(Indicador indicador);
    Task<bool> UpdateAsync(Indicador indicador);
    Task<bool> DeleteAsync(int id);
    Task<bool> InactivarAsync(int id);
    Task<bool> ActivarAsync(int id);
}
