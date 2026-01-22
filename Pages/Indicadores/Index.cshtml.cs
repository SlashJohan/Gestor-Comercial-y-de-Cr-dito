using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Indicadores;

public class IndexModel : PageModel
{
    private readonly IIndicadorRepository _indicadorRepository;

    public IndexModel(IIndicadorRepository indicadorRepository)
    {
        _indicadorRepository = indicadorRepository;
    }

    public IEnumerable<Indicador> Indicadores { get; set; } = Enumerable.Empty<Indicador>();

    public async Task OnGetAsync()
    {
        try
        {
            Indicadores = await _indicadorRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar indicadores: {ex.Message}";
            Indicadores = Enumerable.Empty<Indicador>();
        }
    }
}
