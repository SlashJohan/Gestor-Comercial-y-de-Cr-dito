using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Companies;

public class EditModel : PageModel
{
    private readonly IEmpresaRepository _empresaRepository;

    public EditModel(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

    [BindProperty]
    public Models.Empresa Empresa { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var empresa = await _empresaRepository.GetByIdAsync(id.Value);
        if (empresa == null)
        {
            return NotFound();
        }

        Empresa = empresa;
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
            var updated = await _empresaRepository.UpdateAsync(Empresa);
            if (!updated)
            {
                TempData["ErrorMessage"] = "No se pudo actualizar la empresa.";
                return Page();
            }

            TempData["SuccessMessage"] = "Empresa actualizada exitosamente.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al actualizar la empresa: {ex.Message}";
            return Page();
        }
    }
}
