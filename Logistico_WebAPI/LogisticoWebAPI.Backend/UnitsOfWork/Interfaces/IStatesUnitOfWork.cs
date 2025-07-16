using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface IStatesUnitOfWork
    {
        Task<ActionResponse<IEnumerable<State>>> GetAsync();

        Task<ActionResponse<State>> GetAsync(int id);

        Task<IEnumerable<State>> GetComboAsync();
    }
}