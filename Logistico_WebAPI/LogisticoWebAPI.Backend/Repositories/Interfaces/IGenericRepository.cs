using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<ActionResponses<T>> GetAsync(int id);

        Task<ActionResponses<IEnumerable<T>>> GetAllAsync();

        Task<ActionResponses<T>> PostAsync(T entity);

        Task<ActionResponses<T>> PutAsync(T entity);

        Task<ActionResponses<T>> DeleteAsync(int id);
    }
}