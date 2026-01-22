using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Companies;

public class CreateModel : PageModel
{
    private readonly IEmpresaRepository _empresaRepository;

    public CreateModel(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

    [BindProperty]
    public Empresa Empresa { get; set; } = new();

    public IActionResult OnGet()
    {
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
            await _empresaRepository.CreateAsync(Empresa);
            TempData["SuccessMessage"] = "Empresa creada exitosamente.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al crear la empresa: {ex.Message}";
            return Page();
        }
    }
}
