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

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                query = query.Where(f => f.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Event>>
            {
                WasSuccess = true,
                Result = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var query = _context.Events.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                query = query.Where(f => f.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }
    }
} 
