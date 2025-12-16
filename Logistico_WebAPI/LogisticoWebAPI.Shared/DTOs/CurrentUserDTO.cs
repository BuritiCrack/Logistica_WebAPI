namespace LogisticoWebAPI.Shared.DTOs
{
    public class CurrentUserDTO
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserType { get; set; } = null!;
    }
}