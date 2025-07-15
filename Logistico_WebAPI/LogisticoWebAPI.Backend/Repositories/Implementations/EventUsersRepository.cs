using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using LogisticoWebAPI.Shared.Extensions;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ActionResponses<ApplyToEventDTO>> ApplyToEventAsync(string email, ApplyToEventDTO applyToEventDTO)
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == applyToEventDTO.EventId);
            if (eventEntity == null)
            {
                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = false,
                    Message = "El evento especificado no existe."
                };
            }

            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = false,
                    Message = "El usuario especificado no existe."
                };
            }else if (!user.IsActive)
            {
                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = false,
                    Message = "El usuario no está activo. No puede aplicar a eventos."
                };
            }

            if (eventEntity.StartDate <= DateTime.UtcNow)
            {
                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = false,
                    Message = "No puedes aplicar a un evento que ya ha comenzado."
                };
            }

            //verificar que el usuario no haya aplicado al evento
            var hasApplied = await _context.EventUsers
                .AnyAsync(eu => eu.User!.Email == email && eu.EventId == eventEntity.Id);
            if (hasApplied)
            {
                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = false,
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

                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = true,
                    Result = applyToEventDTO
                };
            }
            catch (Exception ex)
            {
                return new ActionResponses<ApplyToEventDTO>
                {
                    WassSuccess = false,
                    Message = ex.Message
                };
            }

        }

        public async Task<ActionResponses<UpdateApplicationStatusDTO>> UpdateApplicationStatusAsync(string email, UpdateApplicationStatusDTO updateApplicationStatusDTO)
        {
            var application = await _context.EventUsers
                .FirstOrDefaultAsync(eu => eu.Id == updateApplicationStatusDTO.ApplicationId);
            if (application == null)
            {
                return new ActionResponses<UpdateApplicationStatusDTO>
                {
                    WassSuccess = false,
                    Message = "La postulación especificada no existe."
                };
            }

            // Verificar que el estado sea válido para cambio por admin
            if (application.Status == ApplicationStatus.CancelledByUser)
            {
                return new ActionResponses<UpdateApplicationStatusDTO>
                {
                    WassSuccess = false,
                    Message = "No se puede cambiar el estado de una aplicación cancelada por el usuario."
                };
            }
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponses<UpdateApplicationStatusDTO>
                {
                    WassSuccess = false,
                    Message = "El usuario especificado no existe."
                };
            }
                try
            {
                application.Status = updateApplicationStatusDTO.NewStatus;
                application.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ActionResponses<UpdateApplicationStatusDTO>
                {
                    WassSuccess = true,
                    Result = updateApplicationStatusDTO
                };
            }
            catch (Exception ex)
            {
                return new ActionResponses<UpdateApplicationStatusDTO>
                {
                    WassSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponses<EventUser>> CancelApplicationAsync(string email, int eventId)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponses<EventUser>
                {
                    WassSuccess = false,
                    Message = "El usuario especificado no existe."
                };
            }
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity == null)
            {
                return new ActionResponses<EventUser>
                {
                    WassSuccess = false,
                    Message = "El evento especificado no existe."
                };
            }

            var application = await _context.EventUsers
                .Include(eu => eu.Event)
                .Include(eu => eu.User)
                .FirstOrDefaultAsync(eu => eu.User!.Email == email && eu.EventId == eventEntity.Id);

            if (application == null)
            {
                return new ActionResponses<EventUser>
                {
                    WassSuccess = false,
                    Message = "No se encontró una postulación para este evento."
                };
            }

            // Solo permitir cancelar si está pendiente o aceptada
            if (application.Status == ApplicationStatus.CancelledByUser)
            {
                return new ActionResponses<EventUser>
                {
                    WassSuccess = false,
                    Message = "Esta aplicación ya fue cancelada anteriormente."
                };
            }

            try
            {
                application.Status = ApplicationStatus.CancelledByUser;
                application.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ActionResponses<EventUser>
                {
                    WassSuccess = true,
                    Result = application,
                    Message = "Aplicación cancelada exitosamente."
                };
            }
            catch (Exception ex)
            {
                return new ActionResponses<EventUser>
                {
                    WassSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponses<IEnumerable<EventUser>>> GetUserApplicationsAsync(string email)
        {
            var applications = await _context.EventUsers
                .Include(eu => eu.Event)
                .Include(eu => eu.User)
                .Where(eu => eu.User!.Email == email)
                .OrderBy(eu => eu.Status == ApplicationStatus.CancelledByUser)
                .ThenByDescending(eu => eu.LastUpdated)
                .ToListAsync();

            return new ActionResponses<IEnumerable<EventUser>>
            {
                WassSuccess = true,
                Result = applications
            };
        }

        public async Task<ActionResponses<IEnumerable<EventUser>>> GetEventApplicationsAsync(int eventId)
        {
            var applications = await _context.EventUsers
                .Include(eu => eu.Event)
                .Include(eu => eu.User)
                .Where(eu => eu.EventId == eventId)
                .OrderBy(eu => eu.Status == ApplicationStatus.CancelledByUser)
                .ThenBy(eu => eu.RegistrationDate)
                .ToListAsync();

            return new ActionResponses<IEnumerable<EventUser>>
            {
                WassSuccess = true,
                Result = applications
            };
        }

        public async Task<ActionResponses<EventUser>> GetApplicationAsync(int applicationId)
        {
            try
            {
                var application = await _context.EventUsers
                    .Include(eu => eu.Event)
                    .Include(eu => eu.User)
                    .FirstOrDefaultAsync(eu => eu.Id == applicationId);

                if (application == null)
                {
                    return new ActionResponses<EventUser>
                    {
                        WassSuccess = false,
                        Message = "Aplicación no encontrada."
                    };
                }

                return new ActionResponses<EventUser>
                {
                    WassSuccess = true,
                    Result = application
                };
            }
            catch (Exception ex)
            {
                return new ActionResponses<EventUser>
                {
                    WassSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> HasUserAppliedToEventAsync(string email, int eventId)
        {
            return await _context.EventUsers
                .AnyAsync(eu => eu.User!.Email == email && eu.EventId == eventId);
        }
    }
}