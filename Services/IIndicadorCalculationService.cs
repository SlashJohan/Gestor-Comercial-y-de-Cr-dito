namespace GestorComercialCredito.Web.Services;

public interface IIndicadorCalculationService
{
    /// <summary>
    /// Calcula indicadores para las combinaciones (EmpresaId, PeriodoId) dadas
    /// y los inserta en ResultadoIndicador vía sp_CalcularIndicador.
    /// </summary>
    Task CalcularIndicadoresParaEmpresasYPeriodosAsync(
        IReadOnlyList<(int EmpresaId, int PeriodoId)> empresaPeriodos,
        CancellationToken ct = default);
}
