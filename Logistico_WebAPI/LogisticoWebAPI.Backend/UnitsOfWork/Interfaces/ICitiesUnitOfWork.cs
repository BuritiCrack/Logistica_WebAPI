using LogisticoWebAPI.Shared.Entities;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface ICitiesUnitOfWork
    {
        Task<IEnumerable<City>> GetComboAsync(int stateId);
    }
}
