using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
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
        public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "El registro no fue encontrado."
                };
            }
            try
            {
                _entity.Remove(row);
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true     
                };
            }
            catch 
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "No se pudo eliminar, porque tiene registros relacionados."
                };
            }
        }

        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<T>>
            {
                WasSuccess = true,
                Result = await _entity.ToListAsync()
            };
        }

        public virtual async Task<ActionResponse<T>> GetAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "El registro no fue encontrado."
                };
            }

            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = row
            };
        }

        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();

            return new ActionResponse<IEnumerable<T>>
            {
                WasSuccess = true,
                Result = await queryable
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public virtual async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public virtual async Task<ActionResponse<T>> PostAsync(T entity)
        {
            _context.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
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

                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = $"Ocurrió un error al intentar crear el registro: {ex.Message}"
                };
            }
            
        }

        public virtual async Task<ActionResponse<T>> PutAsync(T entity)
        {
            _context.Update(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
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

                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = $"Ocurrió un error al intentar crear el registro: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return ExceptionActionResponse(ex);
            }
        }

        private ActionResponse<T> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que intentas crear."
            };
        }

        private ActionResponse<T> ExceptionActionResponse(Exception ex)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = $"Ocurrió un error al intentar crear el registro: {ex.Message}"
            };
        }
    }
}
