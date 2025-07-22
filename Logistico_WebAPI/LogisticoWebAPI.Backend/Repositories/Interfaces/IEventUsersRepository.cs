using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IEventUsersRepository
    {
        Task<ActionResponse<EventUser>> ApplyToEventAsync(string email, int eventId);

        Task<ActionResponse<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO);

        Task<ActionResponse<EventUser>> CancelApplicationAsync(string email, int eventId);

        Task<ActionResponse<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email);

        Task<ActionResponse<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId);

        Task<ActionResponse<EventUser>> GetApplicationAsync(int applicationId);

        Task<ActionResponse<IEnumerable<EventUser>>> GetAsync(PaginationDTO pagination, int eventId);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination, int eventId);
    }
}