using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Indicadores;

public class EditModel : PageModel
{
    private readonly IIndicadorRepository _indicadorRepository;

    public EditModel(IIndicadorRepository indicadorRepository)
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _indicadorRepository.UpdateAsync(Indicador);
            if (result)
            {
                TempData["SuccessMessage"] = "Indicador actualizado exitosamente.";
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo actualizar el indicador.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al actualizar el indicador: {ex.Message}");
            return Page();
        }
    }
}
