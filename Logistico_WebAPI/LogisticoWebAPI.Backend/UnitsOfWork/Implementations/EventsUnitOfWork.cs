using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class EventsUnitOfWork : GenericUnitOfWork<Event>, IEventsUnitOfWork
    {
        private readonly IEventsRepository _eventsRepository;

        public EventsUnitOfWork(IGenericRepository<Event> repository, IEventsRepository eventsRepository) : base(repository)
        {
            _eventsRepository = eventsRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Event>>> GetAsync(PaginationDTO pagination)
            => await _eventsRepository.GetAsync(pagination);
    }
}
