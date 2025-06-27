using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
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

        public virtual async Task<ActionResponses<T>> DeleteAsync(int id)
            => await _repository.DeleteAsync(id);

        public virtual async Task<ActionResponses<IEnumerable<T>>> GetAllAsync()
            => await _repository.GetAllAsync();

        public virtual async Task<ActionResponses<T>> GetAsync(int id)
            => await _repository.GetAsync(id);

        public virtual async Task<ActionResponses<T>> PostAsync(T entity)
            => await _repository.PostAsync(entity);

        public virtual async Task<ActionResponses<T>> PutAsync(T entity)
            => await _repository.PutAsync(entity);
    }
}