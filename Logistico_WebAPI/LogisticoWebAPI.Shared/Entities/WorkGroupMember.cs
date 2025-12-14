using System.ComponentModel.DataAnnotations;

namespace LogisticoWebAPI.Shared.Entities
{
    public class WorkGroupMember
    {
        public int Id { get; set; }

        [Display(Name = "Fecha de Asignación")]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public int WorkGroupId { get; set; }
        public WorkGroup? WorkGroup { get; set; }

        public string UserId { get; set; } = null!;
        public User? User { get; set; }
    }
}