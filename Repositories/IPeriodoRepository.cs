using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface IPeriodoRepository
{
    Task<IEnumerable<Periodo>> GetAllAsync();
    Task<Periodo?> GetByIdAsync(int id);
    Task<Periodo?> GetByAnioAsync(int anio);
    Task<int> CreateAsync(Periodo periodo);
    Task<bool> UpdateAsync(Periodo periodo);
    Task<bool> DeleteAsync(int id);
}
