using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class CitiesRepository : GenericRepository<City>, ICitiesRepository
    {
        private readonly DataContext _context;

        public CitiesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetComboAsync(int stateId)
        {
            return await _context.Cities
                .Where(c => c.StateId == stateId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
