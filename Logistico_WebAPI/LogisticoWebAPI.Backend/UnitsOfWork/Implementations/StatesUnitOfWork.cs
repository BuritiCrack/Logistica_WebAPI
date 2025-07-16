using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class StatesUnitOfWork : GenericUnitOfWork<State>, IStatesUnitOfWork
    {
        private readonly IStatesRepository _statesRepository;

        public StatesUnitOfWork(IGenericRepository<State> repository, IStatesRepository statesRepository) : base(repository)
        {
            _statesRepository = statesRepository;
        }

        public override async Task<ActionResponse<IEnumerable<State>>> GetAsync()
            => await _statesRepository.GetAsync();

        public override async Task<ActionResponse<State>> GetAsync(int id)
            => await _statesRepository.GetAsync(id);

        public async Task<IEnumerable<State>> GetComboAsync()
            => await _statesRepository.GetComboAsync();
    }
}
