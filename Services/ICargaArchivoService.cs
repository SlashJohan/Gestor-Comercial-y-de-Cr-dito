namespace GestorComercialCredito.Web.Services;

public interface ICargaArchivoService
{
    /// <summary>
    /// Procesa archivo Excel (.xlsx) o PDF y carga movimientos contables.
    /// Formato esperado: NIT, NombreEmpresa, Anio, CodigoCuenta, Valor (primera fila = encabezados).
    /// </summary>
    Task<CargaArchivoResult> ProcesarArchivoAsync(Stream stream, string fileName, CancellationToken ct = default);
}

public class CargaArchivoResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int FilasInsertadas { get; set; }
    public int FilasOmitidas { get; set; }
    public List<string> Errores { get; set; } = new();
}
