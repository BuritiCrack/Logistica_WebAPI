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

        public async Task<ActionResponses<ApplyToEventDTO>> ApplyToEventAsync(string email, ApplyToEventDTO applyToEventDTO)
            => await _eventUsersRepository.ApplyToEventAsync(email, applyToEventDTO);
        public async Task<ActionResponses<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO)
            => await _eventUsersRepository.UpdateApplicationStatusAsync(email,updateApplicationStatusDTO);

        public async Task<ActionResponses<EventUser>> CancelApplicationAsync(string email, int eventId)
            => await _eventUsersRepository.CancelApplicationAsync(email, eventId);

        public async Task<ActionResponses<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email)
            => await _eventUsersRepository.GetUserApplicationsAsync(email);

        public async Task<ActionResponses<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId)
            => await _eventUsersRepository.GetEventApplicationsAsync(eventId);

        public async Task<ActionResponses<EventUser>> GetApplicationAsync(int applicationId)
            => await _eventUsersRepository.GetApplicationAsync(applicationId);

        public async Task<bool> HasUserAppliedToEventAsync(string email, int eventId)
            => await _eventUsersRepository.HasUserAppliedToEventAsync(email, eventId);
    }
}
