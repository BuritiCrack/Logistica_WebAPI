using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IStatesRepository
    {
        Task<ActionResponses<IEnumerable<State>>> GetAllAsync();
        Task<ActionResponses<State>> GetAsync(int id);
    }
}
