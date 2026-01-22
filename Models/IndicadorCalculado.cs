using System.ComponentModel.DataAnnotations;

namespace GestorComercialCredito.Web.Models
{
    public class ResultadoIndicador
    {
        public int ResultadoId { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [Required]
        [Display(Name = "Periodo")]
        public int PeriodoId { get; set; }

        [Required]
        [Display(Name = "Indicador")]
        public int IndicadorId { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio")]
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        [Display(Name = "Fecha de Cálculo")]
        public DateTime FechaCalculo { get; set; }

        // Navegación
        public Empresa? Empresa { get; set; }
        public Periodo? Periodo { get; set; }
        public Indicador? Indicador { get; set; }
    }
}
