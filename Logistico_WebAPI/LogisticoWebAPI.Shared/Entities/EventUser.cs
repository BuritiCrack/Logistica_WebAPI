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
        public bool IsUserConfirmed => Status == ApplicationStatus.Accepted;
        public bool IsEventCancelled { get; set; }
    }
}