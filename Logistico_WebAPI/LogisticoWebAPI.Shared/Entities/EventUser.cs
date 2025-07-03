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
        public bool IsUserConfirmed { get; set; }
        public bool IsEventCancelled { get; set; }
    }
}