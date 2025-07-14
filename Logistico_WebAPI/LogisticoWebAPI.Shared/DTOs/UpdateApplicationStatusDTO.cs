using LogisticoWebAPI.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.DTOs
{
    public class UpdateApplicationStatusDTO
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "ID de la Aplicación")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nuevo Estado")]
        public ApplicationStatus NewStatus { get; set; }

        [Display(Name = "Comentarios del administrador")]
        [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string? AdminComments { get; set; }
    }
}
