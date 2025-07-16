using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IEventUsersRepository
    {
        Task<ActionResponse<ApplyToEventDTO>> ApplyToEventAsync(string email, ApplyToEventDTO applyToEventDTO);
        Task<ActionResponse<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email,UpdateApplicationStatusDTO updateApplicationStatusDTO);
        Task<ActionResponse<EventUser>> CancelApplicationAsync(string email, int eventId);
        Task<ActionResponse<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email);
        Task<ActionResponse<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId);
        Task<ActionResponse<EventUser>> GetApplicationAsync(int applicationId);
        Task<bool> HasUserAppliedToEventAsync(string email, int eventId);
    }
}
