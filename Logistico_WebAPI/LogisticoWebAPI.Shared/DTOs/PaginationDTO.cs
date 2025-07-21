namespace LogisticoWebAPI.Shared.DTOs
{
    public class PaginationDTO
    {
        public int Id { get; set; }
        public int Page { get; set; } = 1;
        public int RecordsNumber { get; set; } = 12;
        public string? Filter { get; set; }
    }
}