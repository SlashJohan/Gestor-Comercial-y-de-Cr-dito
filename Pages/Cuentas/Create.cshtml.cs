using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Cuentas;

public class CreateModel : PageModel
{
    private readonly ICuentaPUCRepository _cuentaRepository;

    public CreateModel(ICuentaPUCRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    [BindProperty]
    public CuentaPUC Cuenta { get; set; } = new();

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
            await _cuentaRepository.CreateAsync(Cuenta);
            TempData["SuccessMessage"] = "Cuenta creada exitosamente.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear la cuenta: {ex.Message}");
            return Page();
        }
    }
}
