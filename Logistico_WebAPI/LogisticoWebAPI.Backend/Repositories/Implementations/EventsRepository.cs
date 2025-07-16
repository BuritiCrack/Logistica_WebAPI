using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class EventsRepository : GenericRepository<Event>, IEventsRepository
    {
        private readonly DataContext _context;

        public EventsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<Event>>> GetAsync(PaginationDTO pagination)
        {
            var query = _context.Events.AsQueryable();

            return new ActionResponse<IEnumerable<Event>>
            {
                WasSuccess = true,
                Result = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}
