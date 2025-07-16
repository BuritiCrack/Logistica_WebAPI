using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class EventUsersUnitOfWork : IEventUsersUnitOfWork
    {
        private readonly IEventUsersRepository _eventUsersRepository;

        public EventUsersUnitOfWork(IEventUsersRepository eventUsersRepository)
        {
            _eventUsersRepository = eventUsersRepository;
        }

        public async Task<ActionResponse<ApplyToEventDTO>> ApplyToEventAsync(string email, ApplyToEventDTO applyToEventDTO)
            => await _eventUsersRepository.ApplyToEventAsync(email, applyToEventDTO);
        public async Task<ActionResponse<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO)
            => await _eventUsersRepository.UpdateApplicationStatusAsync(email,updateApplicationStatusDTO);

        public async Task<ActionResponse<EventUser>> CancelApplicationAsync(string email, int eventId)
            => await _eventUsersRepository.CancelApplicationAsync(email, eventId);

        public async Task<ActionResponse<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email)
            => await _eventUsersRepository.GetUserApplicationsAsync(email);

        public async Task<ActionResponse<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId)
            => await _eventUsersRepository.GetEventApplicationsAsync(eventId);

        public async Task<ActionResponse<EventUser>> GetApplicationAsync(int applicationId)
            => await _eventUsersRepository.GetApplicationAsync(applicationId);

        public async Task<bool> HasUserAppliedToEventAsync(string email, int eventId)
            => await _eventUsersRepository.HasUserAppliedToEventAsync(email, eventId);
    }
}
