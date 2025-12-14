using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.DTOs
{
    public class WorkGroupDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Display(Name = "Nombre del Grupo")]
        public string Name { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Evento")]
        public int EventId { get; set; }


        public string? CoordinatorName { get; set; }
        public string? EventName { get; set; }
    }
}