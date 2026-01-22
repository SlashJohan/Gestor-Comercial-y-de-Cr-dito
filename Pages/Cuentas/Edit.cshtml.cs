using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Cuentas;

public class EditModel : PageModel
{
    private readonly ICuentaPUCRepository _cuentaRepository;

    public EditModel(ICuentaPUCRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    [BindProperty]
    public CuentaPUC Cuenta { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cuenta = await _cuentaRepository.GetByIdAsync(id.Value);
        if (cuenta == null)
        {
            return NotFound();
        }

        Cuenta = cuenta;
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
            var result = await _cuentaRepository.UpdateAsync(Cuenta);
            if (result)
            {
                TempData["SuccessMessage"] = "Cuenta actualizada exitosamente.";
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo actualizar la cuenta.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al actualizar la cuenta: {ex.Message}");
            return Page();
        }
    }
}
