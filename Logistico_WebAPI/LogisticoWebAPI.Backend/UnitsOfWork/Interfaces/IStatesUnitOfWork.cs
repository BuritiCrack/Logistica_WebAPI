using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface IStatesUnitOfWork
    {
        Task<ActionResponses<IEnumerable<State>>> GetAllAsync();

        Task<ActionResponses<State>> GetAsync(int id);

        Task<IEnumerable<State>> GetComboAsync();
    }
}