using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(DataContext context)
        {
            _context = context;
            _entity = context.Set<T>();
        }
        public virtual async Task<ActionResponses<T>> DeleteAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponses<T>
                {
                    WassSuccess = false,
                    Message = "El registro no fue encontrado."
                };
            }
            try
            {
                _entity.Remove(row);
                await _context.SaveChangesAsync();
                return new ActionResponses<T>
                {
                    WassSuccess = true     
                };
            }
            catch 
            {
                return new ActionResponses<T>
                {
                    WassSuccess = false,
                    Message = "No se pudo eliminar, porque tiene registros relacionados."
                };
            }
        }

        public virtual async Task<ActionResponses<IEnumerable<T>>> GetAllAsync()
        {
            return new ActionResponses<IEnumerable<T>>
            {
                WassSuccess = true,
                Result = await _entity.ToListAsync()
            };
        }

        public virtual async Task<ActionResponses<T>> GetAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponses<T>
                {
                    WassSuccess = false,
                    Message = "El registro no fue encontrado."
                };
            }

            return new ActionResponses<T>
            {
                WassSuccess = true,
                Result = row
            };
        }

        public virtual async Task<ActionResponses<T>> PostAsync(T entity)
        {
            _context.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponses<T>
                {
                    WassSuccess = true,
                    Result = entity,
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate key"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponses<T>
                {
                    WassSuccess = false,
                    Message = $"Ocurrió un error al intentar crear el registro: {ex.Message}"
                };
            }
            
        }

        public virtual async Task<ActionResponses<T>> PutAsync(T entity)
        {
            _context.Update(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponses<T>
                {
                    WassSuccess = true,
                    Result = entity,
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate key"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponses<T>
                {
                    WassSuccess = false,
                    Message = $"Ocurrió un error al intentar crear el registro: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return ExceptionActionResponse(ex);
            }
        }

        private ActionResponses<T> DbUpdateExceptionActionResponse()
        {
            return new ActionResponses<T>
            {
                WassSuccess = false,
                Message = "Ya existe el registro que intentas crear."
            };
        }

        private ActionResponses<T> ExceptionActionResponse(Exception ex)
        {
            return new ActionResponses<T>
            {
                WassSuccess = false,
                Message = $"Ocurrió un error al intentar crear el registro: {ex.Message}"
            };
        }
    }
}
