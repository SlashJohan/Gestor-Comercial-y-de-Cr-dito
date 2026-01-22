using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Cuentas;

public class IndexModel : PageModel
{
    private readonly ICuentaPUCRepository _cuentaRepository;

    public IndexModel(ICuentaPUCRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    public IEnumerable<CuentaPUC> Cuentas { get; set; } = Enumerable.Empty<CuentaPUC>();

    public async Task OnGetAsync()
    {
        try
        {
            Cuentas = await _cuentaRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar cuentas: {ex.Message}";
            Cuentas = Enumerable.Empty<CuentaPUC>();
        }
    }

    public async Task<IActionResult> OnPostInactivarAsync(int id)
    {
        var result = await _cuentaRepository.InactivarAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Cuenta inactivada correctamente.";
        }
        else
        {
            TempData["ErrorMessage"] = "No se pudo inactivar la cuenta.";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostActivarAsync(int id)
    {
        var result = await _cuentaRepository.ActivarAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Cuenta activada correctamente.";
        }
        else
        {
            TempData["ErrorMessage"] = "No se pudo activar la cuenta.";
        }
        return RedirectToPage();
    }
}
