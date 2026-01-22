using System.ComponentModel.DataAnnotations;

namespace GestorComercialCredito.Web.Models
{
    public class IndicadorFormula
    {
        public int FormulaId { get; set; }

        [Required]
        [Display(Name = "Indicador")]
        public int IndicadorId { get; set; }

        [Required(ErrorMessage = "La fórmula SQL es obligatoria")]
        [StringLength(500, ErrorMessage = "La fórmula no puede exceder 500 caracteres")]
        [Display(Name = "Fórmula SQL")]
        public string FormulaSQL { get; set; } = string.Empty;

        // Navegación
        public Indicador? Indicador { get; set; }
    }
}
