using System.ComponentModel.DataAnnotations;

namespace GestorComercialCredito.Web.Models
{
    public class Periodo
    {
        public int PeriodoId { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Display(Name = "Año")]
        public int Anio { get; set; }
    }
}
