using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface IEventUsersUnitOfWork
    {
        Task<ActionResponses<ApplyToEventDTO>> ApplyToEventAsync(string email, ApplyToEventDTO applyToEventDTO);
        Task<ActionResponses<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO);
        Task<ActionResponses<EventUser>> CancelApplicationAsync(string email, int eventId);
        Task<ActionResponses<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email);
        Task<ActionResponses<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId);
        Task<ActionResponses<EventUser>> GetApplicationAsync(int applicationId);
        Task<bool> HasUserAppliedToEventAsync(string email, int eventId);
    }
}
