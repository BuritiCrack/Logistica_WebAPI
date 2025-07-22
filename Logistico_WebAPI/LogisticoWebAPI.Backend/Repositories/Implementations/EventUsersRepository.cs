using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class EventUsersRepository : GenericRepository<EventUser>, IEventUsersRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public EventUsersRepository(DataContext context, IUsersRepository usersRepository) : base(context)

        {
            _context = context;
            _usersRepository = usersRepository;
        }

        public async Task<ActionResponse<EventUser>> ApplyToEventAsync(string email, int eventId)
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity == null)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "El evento especificado no existe."
                };
            }

            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "El usuario especificado no existe."
                };
            }
            else if (!user.IsActive)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "Estas inactivo. No puede aplicar a eventos."
                };
            }

            if (eventEntity.StartDate <= DateTime.UtcNow)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "No puedes aplicar a un evento que ya ha comenzado."
                };
            }

            //verificar que el usuario no haya aplicado al evento
            var hasApplied = await _context.EventUsers
                .AnyAsync(eu => eu.User!.Email == email && eu.EventId == eventId);
            if (hasApplied)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "Ya has aplicado a este evento."
                };
            }

            var eventUser = new EventUser
            {
                User = user,
                Event = eventEntity
            };

            try
            {
                _context.Add(eventUser);
                await _context.SaveChangesAsync();

                return new ActionResponse<EventUser>
                {
                    WasSuccess = true,
                    Result = eventUser
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO)
        {
            var application = await _context.EventUsers
                .FirstOrDefaultAsync(eu => eu.Id == updateApplicationStatusDTO.ApplicationId);
            if (application == null)
            {
                return new ActionResponse<UpdateApplicationStatusDTO>
                {
                    WasSuccess = false,
                    Message = "La postulación especificada no existe."
                };
            }

            // Verificar que el estado sea válido para cambio por admin
            if (application.Status == ApplicationStatus.CancelledByUser)
            {
                return new ActionResponse<UpdateApplicationStatusDTO>
                {
                    WasSuccess = false,
                    Message = "No se puede cambiar el estado de una aplicación cancelada por el usuario."
                };
            }
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<UpdateApplicationStatusDTO>
                {
                    WasSuccess = false,
                    Message = "El usuario especificado no existe."
                };
            }
            try
            {
                application.Status = updateApplicationStatusDTO.NewStatus;
                application.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ActionResponse<UpdateApplicationStatusDTO>
                {
                    WasSuccess = true,
                    Result = updateApplicationStatusDTO
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<UpdateApplicationStatusDTO>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<EventUser>> CancelApplicationAsync(string email, int eventId)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "El usuario especificado no existe."
                };
            }
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity == null)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "El evento especificado no existe."
                };
            }

            var application = await _context.EventUsers
                .Include(eu => eu.Event)
                .Include(eu => eu.User)
                .FirstOrDefaultAsync(eu => eu.User!.Email == email && eu.EventId == eventEntity.Id);

            if (application == null)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "No se encontró una postulación para este evento."
                };
            }

            // Solo permitir cancelar si está pendiente o aceptada
            if (application.Status == ApplicationStatus.CancelledByUser)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = "Esta aplicación ya fue cancelada anteriormente."
                };
            }

            try
            {
                application.Status = ApplicationStatus.CancelledByUser;
                application.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ActionResponse<EventUser>
                {
                    WasSuccess = true,
                    Result = application,
                    Message = "Aplicación cancelada exitosamente."
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email)
        {
            var applications = await _context.EventUsers
                .Include(eu => eu.Event)
                .Include(eu => eu.User)
                .Where(eu => eu.User!.Email == email && eu.LastUpdated > DateTime.UtcNow.AddDays(-20))
                .OrderByDescending(eu => eu.LastUpdated)
                .ThenByDescending(eu => eu.Status == ApplicationStatus.Accepted)
                .ToListAsync();

            return new ActionResponse<IEnumerable<EventUser>>
            {
                WasSuccess = true,
                Result = applications
            };
        }

        public async Task<ActionResponse<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId)
        {
            var applications = await _context.EventUsers
                .Include(eu => eu.Event)
                .Include(eu => eu.User)
                .Where(eu => eu.EventId == eventId)
                .OrderBy(eu => eu.Status == ApplicationStatus.CancelledByUser)
                .ThenBy(eu => eu.RegistrationDate)
                .ToListAsync();

            return new ActionResponse<IEnumerable<EventUser>>
            {
                WasSuccess = true,
                Result = applications
            };
        }

        public async Task<ActionResponse<EventUser>> GetApplicationAsync(int applicationId)
        {
            try
            {
                var application = await _context.EventUsers
                    .Include(eu => eu.Event)
                    .Include(eu => eu.User)
                    .FirstOrDefaultAsync(eu => eu.Id == applicationId);

                if (application == null)
                {
                    return new ActionResponse<EventUser>
                    {
                        WasSuccess = false,
                        Message = "Aplicación no encontrada."
                    };
                }

                return new ActionResponse<EventUser>
                {
                    WasSuccess = true,
                    Result = application
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<EventUser>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination,int eventId)
        {
            var query = _context.EventUsers
                .Where(eu => eu.EventId == eventId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                query = query.Where(f => f.User!.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                         f.User.LastName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                         f.User.Document.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<IEnumerable<EventUser>>> GetAsync(PaginationDTO pagination, int eventId)
        {
            var query = _context.EventUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                query = query.Where(f => f.User!.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                         f.User.LastName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                         f.User.Document.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<EventUser>>
            {
                WasSuccess = true,
                Result = await query
                    .Include(eu => eu.Event)
                    .Include(eu => eu.User)
                    .Where(eu => eu.EventId == eventId)
                    .OrderBy(eu => eu.Status == ApplicationStatus.CancelledByUser)
                    .ThenBy(eu => eu.RegistrationDate)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }
    }
}