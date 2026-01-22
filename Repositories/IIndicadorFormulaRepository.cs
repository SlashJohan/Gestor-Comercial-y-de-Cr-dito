using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface IIndicadorFormulaRepository
{
    Task<IEnumerable<IndicadorFormula>> GetAllAsync();
    Task<IndicadorFormula?> GetByIdAsync(int id);
    Task<IndicadorFormula?> GetByIndicadorIdAsync(int indicadorId);
    Task<int> CreateAsync(IndicadorFormula formula);
    Task<bool> UpdateAsync(IndicadorFormula formula);
    Task<bool> DeleteAsync(int id);
}
