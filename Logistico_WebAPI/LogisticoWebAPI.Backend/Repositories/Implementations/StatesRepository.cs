using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class StatesRepository : GenericRepository<State>, IStatesRepository
    {
        private readonly DataContext _context;

        public StatesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponses<IEnumerable<State>>> GetAllAsync()
        {
            var states = await _context.States
                .Include(s => s.Cities)
                .ToListAsync();
            return new ActionResponses<IEnumerable<State>>
            {
                WassSuccess = true,
                Result = states
            };
        }

        public override async Task<ActionResponses<State>> GetAsync(int id)
        {
            var state = await _context.States
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return new ActionResponses<State>
                {
                    WassSuccess = false,
                    Message = "El departamento no fue encontrado."
                };
            }
            return new ActionResponses<State>
            {
                WassSuccess = true,
                Result = state
            };
        }
    }
}
