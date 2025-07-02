using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class CitiesUnitOfWork : GenericUnitOfWork<City>, ICitiesUnitOfWork
    {
        private readonly ICitiesRepository _citiesRepository;

        public CitiesUnitOfWork(IGenericRepository<City> repository, ICitiesRepository citiesRepository) : base(repository)
        { 
            _citiesRepository = citiesRepository;
        }

        public async Task<IEnumerable<City>> GetComboAsync(int stateId)
            => await  _citiesRepository.GetComboAsync(stateId);
    }
}
