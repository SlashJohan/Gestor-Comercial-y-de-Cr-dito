using GestorComercialCredito.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.Companies;

public class DeleteModel : PageModel
{
    private readonly IEmpresaRepository _empresaRepository;

    public DeleteModel(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var deleted = await _empresaRepository.DeleteAsync(id.Value);
        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToPage("./Index");
    }
}
