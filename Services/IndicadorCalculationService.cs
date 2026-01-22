using GestorComercialCredito.Web.Repositories;

namespace GestorComercialCredito.Web.Services;

public class IndicadorCalculationService : IIndicadorCalculationService
{
    private readonly IIndicadorRepository _indicadorRepository;
    private readonly IIndicadorFormulaRepository _formulaRepository;
    private readonly IResultadoIndicadorRepository _resultadoRepository;
    private readonly IStoredProcedureService _spService;

    public IndicadorCalculationService(
        IIndicadorRepository indicadorRepository,
        IIndicadorFormulaRepository formulaRepository,
        IResultadoIndicadorRepository resultadoRepository,
        IStoredProcedureService spService)
    {
        _indicadorRepository = indicadorRepository;
        _formulaRepository = formulaRepository;
        _resultadoRepository = resultadoRepository;
        _spService = spService;
    }

    public async Task CalcularIndicadoresParaEmpresasYPeriodosAsync(
        IReadOnlyList<(int EmpresaId, int PeriodoId)> empresaPeriodos,
        CancellationToken ct = default)
    {
        var formulas = await _formulaRepository.GetAllAsync();
        var indicadoresConFormula = formulas
            .Where(f => f.Indicador != null && f.Indicador.Activo)
            .Select(f => f.IndicadorId)
            .Distinct()
            .ToList();
        if (indicadoresConFormula.Count == 0) return;

        foreach (var (empresaId, periodoId) in empresaPeriodos)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                await _resultadoRepository.DeleteByEmpresaAndPeriodoAsync(empresaId, periodoId);
            }
            catch { /* ignorar si no hay resultados previos */ }

            foreach (var indicadorId in indicadoresConFormula)
            {
                if (ct.IsCancellationRequested) break;
                try
                {
                    await _spService.ExecuteStoredProcedureNonQueryAsync(
                        "sp_CalcularIndicador",
                        new { EmpresaId = empresaId, PeriodoId = periodoId, IndicadorId = indicadorId });
                }
                catch (Exception)
                {
                    // un indicador puede fallar por fórmula o datos; continuar con el resto
                }
            }
        }
    }
}
