using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Companies;

public class IndexModel : PageModel
{
    private readonly IEmpresaRepository _empresaRepository;

    public IndexModel(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

    public IEnumerable<Empresa> Empresas { get; set; } = Enumerable.Empty<Empresa>();

    public async Task OnGetAsync()
    {
        try
        {
            var empresasResult = await _empresaRepository.GetAllAsync();
            
            if (empresasResult == null)
            {
                Empresas = Enumerable.Empty<Empresa>();
                TempData["WarningMessage"] = "No se pudieron cargar las empresas (resultado null).";
                return;
            }

            Empresas = empresasResult;
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error al cargar empresas: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $" Detalles: {ex.InnerException.Message}";
            }
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                errorMessage += $" StackTrace: {ex.StackTrace.Substring(0, Math.Min(500, ex.StackTrace.Length))}";
            }
            TempData["ErrorMessage"] = errorMessage;
            Empresas = Enumerable.Empty<Empresa>();
        }
    }

    public async Task<IActionResult> OnPostInactivarAsync(int id)
    {
        var result = await _empresaRepository.InactivarAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Empresa inactivada correctamente.";
        }
        else
        {
            TempData["ErrorMessage"] = "No se pudo inactivar la empresa.";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostActivarAsync(int id)
    {
        var result = await _empresaRepository.ActivarAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Empresa activada correctamente.";
        }
        else
        {
            TempData["ErrorMessage"] = "No se pudo activar la empresa.";
        }
        return RedirectToPage();
    }
}
