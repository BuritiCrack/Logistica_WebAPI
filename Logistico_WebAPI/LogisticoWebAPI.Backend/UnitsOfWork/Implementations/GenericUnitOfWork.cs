using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class GenericUnitOfWork<T> : IGenericUnitOfWork<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericUnitOfWork(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
            => await _repository.DeleteAsync(id);

        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync()
            => await _repository.GetAsync();

        public virtual async Task<ActionResponse<T>> GetAsync(int id)
            => await _repository.GetAsync(id);

        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination)
            => await _repository.GetAsync(pagination);

        public virtual Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
            => _repository.GetTotalPagesAsync(pagination);

        public virtual async Task<ActionResponse<T>> PostAsync(T entity)
            => await _repository.PostAsync(entity);

        public virtual async Task<ActionResponse<T>> PutAsync(T entity)
            => await _repository.PutAsync(entity);
    }
}