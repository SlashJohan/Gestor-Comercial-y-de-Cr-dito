using System.ComponentModel.DataAnnotations;

namespace GestorComercialCredito.Web.Models
{
    public class CuentaPUC
    {
        public int CuentaId { get; set; }

        [Required(ErrorMessage = "El código de cuenta es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de cuenta es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        [Display(Name = "Tipo de Cuenta")]
        public string TipoCuenta { get; set; } = string.Empty; // ACTIVO_CORRIENTE, PASIVO_CORRIENTE, INGRESO, UTILIDAD_NETA, etc.

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;
    }
}
