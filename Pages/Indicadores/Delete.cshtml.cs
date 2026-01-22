using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Indicadores;

public class DeleteModel : PageModel
{
    private readonly IIndicadorRepository _indicadorRepository;

    public DeleteModel(IIndicadorRepository indicadorRepository)
    {
        _indicadorRepository = indicadorRepository;
    }

    [BindProperty]
    public Indicador Indicador { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var indicador = await _indicadorRepository.GetByIdAsync(id.Value);
        if (indicador == null)
        {
            return NotFound();
        }

        Indicador = indicador;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var result = await _indicadorRepository.DeleteAsync(id.Value);
            if (result)
            {
                TempData["SuccessMessage"] = "Indicador eliminado exitosamente.";
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo eliminar el indicador.";
                return RedirectToPage("./Index");
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar el indicador: {ex.Message}";
            return RedirectToPage("./Index");
        }
    }
}
