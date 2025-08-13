using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface IEventUsersUnitOfWork
    {
        Task<ActionResponse<EventUser>> ApplyToEventAsync(string email, int eventId);

        Task<ActionResponse<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO);

        Task<ActionResponse<AttendDTO>> DidUserAttend(string email, AttendDTO attendDTO);

        Task<ActionResponse<EventUser>> CancelApplicationAsync(string email, int eventId);

        Task<ActionResponse<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email);

        Task<ActionResponse<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId);

        Task<ActionResponse<EventUser>> GetApplicationAsync(int applicationId);

        Task<ActionResponse<IEnumerable<EventUser>>> GetAsync(PaginationDTO pagination, int eventId);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination, int eventId);

        Task<ActionResponse<EventStatisticsDTO>> GetEventStatisticsAsync(int eventId);
    }
}