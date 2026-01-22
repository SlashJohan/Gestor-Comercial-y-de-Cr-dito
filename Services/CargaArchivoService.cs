using System.Globalization;
using ClosedXML.Excel;
using GestorComercialCredito.Web.Models;
using GestorComercialCredito.Web.Repositories;
using UglyToad.PdfPig;

namespace GestorComercialCredito.Web.Services;

public class CargaArchivoService : ICargaArchivoService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPeriodoRepository _periodoRepository;
    private readonly ICuentaPUCRepository _cuentaRepository;
    private readonly IMovimientoContableRepository _movimientoRepository;
    private readonly IIndicadorCalculationService _calculationService;

    public CargaArchivoService(
        IEmpresaRepository empresaRepository,
        IPeriodoRepository periodoRepository,
        ICuentaPUCRepository cuentaRepository,
        IMovimientoContableRepository movimientoRepository,
        IIndicadorCalculationService calculationService)
    {
        _empresaRepository = empresaRepository;
        _periodoRepository = periodoRepository;
        _cuentaRepository = cuentaRepository;
        _movimientoRepository = movimientoRepository;
        _calculationService = calculationService;
    }

    public async Task<CargaArchivoResult> ProcesarArchivoAsync(Stream stream, string fileName, CancellationToken ct = default)
    {
        var result = new CargaArchivoResult();
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        List<FilaCarga> filas;
        try
        {
            filas = extension switch
            {
                ".xlsx" => ProcesarExcel(stream),
                ".pdf" => ProcesarPdf(stream),
                _ => throw new NotSupportedException($"Formato no soportado: {extension}. Use .xlsx o .pdf.")
            };
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = "Error al leer el archivo: " + ex.Message;
            result.Errores.Add(ex.ToString());
            return result;
        }

        if (filas.Count == 0)
        {
            result.Success = false;
            result.Message = "No se encontraron filas de datos en el archivo.";
            return result;
        }

        var movimientos = new List<MovimientoContable>();
        var empresasPeriodosCalculo = new HashSet<(int EmpresaId, int PeriodoId)>();

        foreach (var f in filas)
        {
            if (ct.IsCancellationRequested) break;

            if (string.IsNullOrWhiteSpace(f.Nit) || string.IsNullOrWhiteSpace(f.CodigoCuenta) || !f.Anio.HasValue || !f.Valor.HasValue)
            {
                result.FilasOmitidas++;
                continue;
            }

            var empresa = await _empresaRepository.GetByNitAsync(f.Nit);
            if (empresa == null)
            {
                empresa = new Empresa
                {
                    Nit = f.Nit.Trim(),
                    Nombre = string.IsNullOrWhiteSpace(f.NombreEmpresa) ? f.Nit : f.NombreEmpresa.Trim(),
                    Activa = true
                };
                empresa.EmpresaId = await _empresaRepository.CreateAsync(empresa);
            }

            var periodo = await _periodoRepository.GetByAnioAsync(f.Anio.Value);
            if (periodo == null)
            {
                periodo = new Periodo { Anio = f.Anio.Value };
                periodo.PeriodoId = await _periodoRepository.CreateAsync(periodo);
            }

            var cuenta = await _cuentaRepository.GetByCodigoAsync(f.CodigoCuenta);
            if (cuenta == null)
            {
                result.Errores.Add($"Cuenta no encontrada: {f.CodigoCuenta}. Regístrela en Gestión > Cuentas (PUC).");
                result.FilasOmitidas++;
                continue;
            }

            movimientos.Add(new MovimientoContable
            {
                EmpresaId = empresa.EmpresaId,
                PeriodoId = periodo.PeriodoId,
                CuentaId = cuenta.CuentaId,
                Valor = f.Valor.Value
            });
            empresasPeriodosCalculo.Add((empresa.EmpresaId, periodo.PeriodoId));
        }

        if (movimientos.Count == 0)
        {
            result.Success = false;
            result.Message = "No se insertó ningún movimiento. Verifique que las cuentas existan y los datos sean válidos.";
            result.FilasOmitidas = filas.Count;
            return result;
        }

        try
        {
            await _movimientoRepository.CreateRangeAsync(movimientos);
            result.FilasInsertadas = movimientos.Count;
            result.FilasOmitidas = filas.Count - movimientos.Count;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = "Error al guardar movimientos: " + ex.Message;
            result.Errores.Add(ex.ToString());
            return result;
        }

        try
        {
            await _calculationService.CalcularIndicadoresParaEmpresasYPeriodosAsync(empresasPeriodosCalculo.ToList(), ct);
        }
        catch (Exception ex)
        {
            result.Errores.Add("Indicadores: " + ex.Message);
        }

        result.Success = true;
        result.Message = $"Se cargaron {result.FilasInsertadas} movimientos correctamente.";
        if (result.FilasOmitidas > 0)
            result.Message += $" {result.FilasOmitidas} filas omitidas.";
        return result;
    }

    private static List<FilaCarga> ProcesarExcel(Stream stream)
    {
        var filas = new List<FilaCarga>();
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);
        var rows = sheet.RangeUsed()?.Rows() ?? Enumerable.Empty<IXLRangeRow>();
        var list = rows.ToList();
        if (list.Count < 2) return filas;

        var header = list[0];
        var colNIT = BuscarColumna(header, "NIT", "nit");
        var colNombre = BuscarColumna(header, "NombreEmpresa", "Nombre", "nombre", "empresa");
        var colAnio = BuscarColumna(header, "Anio", "Año", "anio", "ano", "year");
        var colCodigo = BuscarColumna(header, "CodigoCuenta", "Codigo", "Código", "Cuenta", "codigo");
        var colValor = BuscarColumna(header, "Valor", "valor", "monto");

        if (colNIT < 0 || colCodigo < 0 || colValor < 0)
            throw new InvalidOperationException("El Excel debe tener columnas NIT, CodigoCuenta (o Código/Cuenta) y Valor. Primera fila = encabezados.");

        for (var i = 1; i < list.Count; i++)
        {
            var row = list[i];
            var nit = GetCellString(row, colNIT);
            var nombre = colNombre >= 0 ? GetCellString(row, colNombre) : null;
            var anio = colAnio >= 0 ? GetCellInt(row, colAnio) : null;
            var codigo = GetCellString(row, colCodigo);
            var valor = GetCellDecimal(row, colValor);
            if (string.IsNullOrWhiteSpace(nit) && string.IsNullOrWhiteSpace(codigo) && !valor.HasValue) continue;
            filas.Add(new FilaCarga { Nit = nit, NombreEmpresa = nombre, Anio = anio, CodigoCuenta = codigo, Valor = valor });
        }

        return filas;
    }

    private static int BuscarColumna(IXLRangeRow header, params string[] names)
    {
        var j = 0;
        foreach (var cell in header.Cells())
        {
            var v = cell.GetString()?.Trim().Replace(" ", "") ?? "";
            foreach (var n in names)
                if (string.Equals(v, n.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                    return j;
            j++;
        }
        return -1;
    }

    private static string? GetCellString(IXLRangeRow row, int col)
    {
        var c = row.Cell(col + 1);
        var v = c.GetString();
        if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
        var n = c.TryGetValue(out double d) ? d : (double?)null;
        return n.HasValue ? n.Value.ToString("G") : null;
    }

    private static int? GetCellInt(IXLRangeRow row, int col)
    {
        var c = row.Cell(col + 1);
        if (c.TryGetValue(out int i)) return i;
        if (c.TryGetValue(out double d) && d >= int.MinValue && d <= int.MaxValue) return (int)d;
        var s = c.GetString();
        return int.TryParse(s, out var parsed) ? parsed : null;
    }

    private static decimal? GetCellDecimal(IXLRangeRow row, int col)
    {
        var c = row.Cell(col + 1);
        if (c.TryGetValue(out decimal m)) return m;
        if (c.TryGetValue(out double d)) return (decimal)d;
        var s = c.GetString();
        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
    }

    private static List<FilaCarga> ProcesarPdf(Stream stream)
    {
        var filas = new List<FilaCarga>();
        using var doc = PdfDocument.Open(stream);
        var allLines = new List<string>();
        
        // Extraer texto de todas las páginas
        foreach (var page in doc.GetPages())
        {
            var words = page.GetWords().ToList();
            if (words.Count == 0) continue;
            
            // Agrupar palabras por línea (misma coordenada Y, tolerancia de 2 píxeles)
            var byLine = words
                .GroupBy(w => Math.Round(w.BoundingBox.Bottom / 2.0) * 2) // Tolerancia de 2 píxeles
                .OrderByDescending(g => g.Key) // De arriba hacia abajo
                .Select(g => string.Join(" ", g.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text.Trim())))
                .Where(line => !string.IsNullOrWhiteSpace(line));
            
            allLines.AddRange(byLine);
        }
        
        if (allLines.Count < 2)
        {
            throw new InvalidOperationException(
                $"El PDF no contiene suficientes líneas de texto ({allLines.Count} encontradas). " +
                "Asegúrate de que el PDF tenga texto extraíble (no sea una imagen escaneada) y contenga una tabla con encabezados: NIT, CodigoCuenta, Valor.");
        }
        
        // Buscar la línea que contiene los encabezados
        int headerIndex = -1;
        char sep = '\t';
        var colNIT = -1; var colNombre = -1; var colAnio = -1; var colCodigo = -1; var colValor = -1;
        
        for (int idx = 0; idx < Math.Min(5, allLines.Count); idx++) // Buscar en las primeras 5 líneas
        {
            var line = allLines[idx];
            var separators = new[] { '\t', ';', ',', '|', ' ' };
            
            foreach (var s in separators)
            {
                var parts = line.Split(s, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) continue;
                
                colNIT = -1; colNombre = -1; colAnio = -1; colCodigo = -1; colValor = -1;
                
                for (var i = 0; i < parts.Length; i++)
                {
                    var p = parts[i].Trim().Replace(" ", "").ToUpperInvariant();
                    if (p.Contains("NIT")) colNIT = i;
                    else if (p.Contains("NOMBRE") || p.Contains("EMPRESA")) colNombre = i;
                    else if (p.Contains("AÑO") || p.Contains("ANIO") || p.Contains("YEAR")) colAnio = i;
                    else if (p.Contains("CODIGO") || p.Contains("CÓDIGO") || p.Contains("CUENTA")) colCodigo = i;
                    else if (p.Contains("VALOR") || p.Contains("MONTO")) colValor = i;
                }
                
                if (colNIT >= 0 && colCodigo >= 0 && colValor >= 0)
                {
                    headerIndex = idx;
                    sep = s;
                    break;
                }
            }
            
            if (headerIndex >= 0) break;
        }
        
        if (headerIndex < 0 || colNIT < 0 || colCodigo < 0 || colValor < 0)
        {
            var sample = allLines.Count > 0 ? allLines[0].Substring(0, Math.Min(100, allLines[0].Length)) : "vacío";
            throw new InvalidOperationException(
                $"No se encontraron los encabezados requeridos (NIT, CodigoCuenta, Valor) en el PDF. " +
                $"Primera línea encontrada: '{sample}...'. " +
                $"El PDF debe tener una tabla con estas columnas en la primera fila.");
        }
        
        // Procesar filas de datos (después del header)
        for (var i = headerIndex + 1; i < allLines.Count; i++)
        {
            var cols = allLines[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (cols.Length <= Math.Max(colNIT, Math.Max(colCodigo, colValor))) continue;
            
            var nit = colNIT < cols.Length ? cols[colNIT].Trim() : "";
            var nombre = colNombre >= 0 && colNombre < cols.Length ? cols[colNombre].Trim() : null;
            var anioS = colAnio >= 0 && colAnio < cols.Length ? cols[colAnio].Trim() : null;
            var codigo = colCodigo < cols.Length ? cols[colCodigo].Trim() : "";
            var valorS = colValor < cols.Length ? cols[colValor].Trim() : "";
            
            int? anio = int.TryParse(anioS, out var a) ? a : null;
            decimal? valor = decimal.TryParse(valorS, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
            
            if (string.IsNullOrWhiteSpace(nit) && string.IsNullOrWhiteSpace(codigo) && !valor.HasValue) continue;
            
            filas.Add(new FilaCarga { Nit = nit, NombreEmpresa = nombre, Anio = anio, CodigoCuenta = codigo, Valor = valor });
        }
        
        return filas;
    }

    private class FilaCarga
    {
        public string? Nit { get; set; }
        public string? NombreEmpresa { get; set; }
        public int? Anio { get; set; }
        public string? CodigoCuenta { get; set; }
        public decimal? Valor { get; set; }
    }
}
