namespace LogisticoWebAPI.Shared.DTOs
{
    public class EventStatisticsDTO
    {
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int AcceptedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int CancelledApplications { get; set; }
    }
}
