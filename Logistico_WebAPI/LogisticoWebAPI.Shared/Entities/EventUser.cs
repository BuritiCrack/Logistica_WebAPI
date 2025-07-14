using LogisticoWebAPI.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.Entities
{
    public class EventUser
    {
        public int Id { get; set; }

        public Event? Event { get; set; }
        public int EventId { get; set; }
        public User? User { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Display(Name ="Estado de la aplicacion")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        
        [Display(Name = "Comentarios del administrador")]
        [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string? AdminComments { get; set; }
        
        [Display(Name = "Fecha de última actualización")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public bool IsUserConfirmed => Status == ApplicationStatus.Accepted;
        public bool IsEventCancelled { get; set; }
    }
}