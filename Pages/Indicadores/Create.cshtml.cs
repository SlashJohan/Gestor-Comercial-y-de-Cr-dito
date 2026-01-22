using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Indicadores;

public class CreateModel : PageModel
{
    private readonly IIndicadorRepository _indicadorRepository;

    public CreateModel(IIndicadorRepository indicadorRepository)
    {
        _indicadorRepository = indicadorRepository;
    }

    [BindProperty]
    public Indicador Indicador { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _indicadorRepository.CreateAsync(Indicador);
            TempData["SuccessMessage"] = "Indicador creado exitosamente.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear el indicador: {ex.Message}");
            return Page();
        }
    }
}
