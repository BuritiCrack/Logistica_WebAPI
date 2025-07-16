using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<ActionResponse<T>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<T>>> GetAsync();

        Task<ActionResponse<T>> PostAsync(T entity);

        Task<ActionResponse<T>> PutAsync(T entity);

        Task<ActionResponse<T>> DeleteAsync(int id);
    }
}