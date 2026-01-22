using GestorComercialCredito.Web.Models;

namespace GestorComercialCredito.Web.Repositories;

public interface IResultadoIndicadorRepository
{
    Task<IEnumerable<ResultadoIndicador>> GetAllAsync();
    Task<IEnumerable<ResultadoIndicador>> GetByFiltrosAsync(int? empresaId, int? indicadorId, int? periodoId);
    Task<ResultadoIndicador?> GetByIdAsync(int id);
    Task<int> CreateAsync(ResultadoIndicador resultadoIndicador);
    Task<bool> UpdateAsync(ResultadoIndicador resultadoIndicador);
    Task<bool> DeleteAsync(int id);
    Task DeleteByEmpresaAndPeriodoAsync(int empresaId, int periodoId);
}
