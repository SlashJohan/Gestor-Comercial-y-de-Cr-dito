using System.ComponentModel.DataAnnotations;

namespace GestorComercialCredito.Web.Models
{
    public class Empresa
    {
        public int EmpresaId { get; set; }

        [Required(ErrorMessage = "El NIT es obligatorio")]
        [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
        [Display(Name = "NIT")]
        public string Nit { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Activa")]
        public bool Activa { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}

