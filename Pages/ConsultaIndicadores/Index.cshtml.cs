using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorComercialCredito.Web.Pages.ConsultaIndicadores;

public class IndexModel : PageModel
{
    private readonly IResultadoIndicadorRepository _resultadoIndicadorRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IIndicadorRepository _indicadorRepository;
    private readonly IPeriodoRepository _periodoRepository;

    public IndexModel(
        IResultadoIndicadorRepository resultadoIndicadorRepository,
        IEmpresaRepository empresaRepository,
        IIndicadorRepository indicadorRepository,
        IPeriodoRepository periodoRepository)
    {
        _resultadoIndicadorRepository = resultadoIndicadorRepository;
        _empresaRepository = empresaRepository;
        _indicadorRepository = indicadorRepository;
        _periodoRepository = periodoRepository;
    }

    [BindProperty(SupportsGet = true)]
    public int? EmpresaId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? IndicadorId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? PeriodoId { get; set; }

    public IEnumerable<ResultadoIndicador> ResultadosIndicadores { get; set; } = Enumerable.Empty<ResultadoIndicador>();
    public SelectList Empresas { get; set; } = null!;
    public SelectList Indicadores { get; set; } = null!;
    public SelectList Periodos { get; set; } = null!;

    public async Task OnGetAsync()
    {
        try
        {
            await LoadSelectListsAsync();
            await LoadResultadosIndicadoresAsync();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar datos: {ex.Message}";
            ResultadosIndicadores = Enumerable.Empty<ResultadoIndicador>();
        }
    }

    public async Task OnPostAsync()
    {
        try
        {
            await LoadSelectListsAsync();
            await LoadResultadosIndicadoresAsync();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar datos: {ex.Message}";
            ResultadosIndicadores = Enumerable.Empty<ResultadoIndicador>();
        }
    }

    private async Task LoadSelectListsAsync()
    {
        var empresas = await _empresaRepository.GetAllAsync();
        Empresas = new SelectList(empresas, "EmpresaId", "Nombre", EmpresaId);

        var indicadores = await _indicadorRepository.GetAllAsync();
        Indicadores = new SelectList(indicadores, "IndicadorId", "Nombre", IndicadorId);

        var periodos = await _periodoRepository.GetAllAsync();
        Periodos = new SelectList(periodos, "PeriodoId", "Anio", PeriodoId);
    }

    private async Task LoadResultadosIndicadoresAsync()
    {
        ResultadosIndicadores = await _resultadoIndicadorRepository.GetByFiltrosAsync(
            EmpresaId, IndicadorId, PeriodoId);
    }
}
