using LogisticoWebAPI.Shared.Entities;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface ICitiesRepository
    {
        Task<IEnumerable<City>> GetComboAsync(int stateId);
    }
}
