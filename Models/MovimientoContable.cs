using System.ComponentModel.DataAnnotations;

namespace GestorComercialCredito.Web.Models
{
    public class MovimientoContable
    {
        public int MovimientoId { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [Required]
        [Display(Name = "Periodo")]
        public int PeriodoId { get; set; }

        [Required]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio")]
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        // Navegación
        public Empresa? Empresa { get; set; }
        public Periodo? Periodo { get; set; }
        public CuentaPUC? Cuenta { get; set; }
    }
}
