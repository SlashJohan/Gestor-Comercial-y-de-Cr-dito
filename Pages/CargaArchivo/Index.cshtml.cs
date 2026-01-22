using GestorComercialCredito.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestorComercialCredito.Web.Pages.CargaArchivo;

public class IndexModel : PageModel
{
    private readonly ICargaArchivoService _cargaArchivoService;

    public IndexModel(ICargaArchivoService cargaArchivoService)
    {
        _cargaArchivoService = cargaArchivoService;
    }

    [BindProperty]
    public IFormFile? Archivo { get; set; }

    public CargaArchivoResult? Resultado { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (Archivo == null || Archivo.Length == 0)
        {
            TempData["ErrorMessage"] = "Seleccione un archivo Excel (.xlsx) o PDF.";
            return RedirectToPage();
        }

        var ext = Path.GetExtension(Archivo.FileName).ToLowerInvariant();
        if (ext != ".xlsx" && ext != ".pdf")
        {
            TempData["ErrorMessage"] = "Solo se permiten archivos .xlsx o .pdf.";
            return RedirectToPage();
        }

        try
        {
            await using var stream = Archivo.OpenReadStream();
            Resultado = await _cargaArchivoService.ProcesarArchivoAsync(stream, Archivo.FileName, ct);
            if (Resultado.Success)
                TempData["SuccessMessage"] = Resultado.Message;
            else
                TempData["ErrorMessage"] = Resultado.Message;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al procesar el archivo: " + ex.Message;
        }

        return Page();
    }
}
