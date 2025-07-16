using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IStatesRepository
    {
        Task<ActionResponse<IEnumerable<State>>> GetAsync();

        Task<ActionResponse<State>> GetAsync(int id);

        Task<IEnumerable<State>> GetComboAsync();
    }
}