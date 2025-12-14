using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.Entities
{
    public class WorkGroup
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo (0) es obligatorio")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Display(Name = "Nombre del Grupo")]
        public string Name { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        // El coordinador del grupo debe de ser un coordinador del evento
        [Required(ErrorMessage = "El campo (0) es obligatorio")]
        [Display(Name = "Coordinador del Grupo")] 
        public string CoordinatorId { get; set; } = null!;

        public User? Coordinator { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public ICollection<WorkGroupMember>? Members { get; set; }

        [Display(Name = "Número de Miembros")]
        public int MembersCount => Members?.Count ?? 0;
    }
}