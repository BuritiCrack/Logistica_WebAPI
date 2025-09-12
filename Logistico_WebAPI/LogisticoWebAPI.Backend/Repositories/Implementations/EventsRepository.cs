using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class EventsRepository : GenericRepository<Event>, IEventsRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public EventsRepository(DataContext context, IFileStorage fileStorage) : base(context)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public override async Task<ActionResponse<Event>> GetAsync(int id)
        {
            try
            {
                var eventEntity = await _context.Events
                    .Include(e => e.EventUsers)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eventEntity == null)
                {
                    return new ActionResponse<Event>
                    {
                        WasSuccess = false,
                        Message = "El evento no fue encontrado."
                    };
                }

                return new ActionResponse<Event>
                {
                    WasSuccess = true,
                    Result = eventEntity
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Event>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<Event>> AddAsync(EventDTO eventDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(eventDTO.Photo))
                {
                    var photoEvent = Convert.FromBase64String(eventDTO.Photo);
                    eventDTO.Photo = await _fileStorage.SaveFileAsync(photoEvent, ".jpg", "events");
                }

                var newEvent = new Event
                {
                    Name = eventDTO.Name,
                    Place = eventDTO.Place,
                    Description = eventDTO.Description,
                    StartDate = eventDTO.StartDate,
                    EndDate = eventDTO.EndDate,
                    PaymentDate = eventDTO.PaymentDate.Date,
                    MealType = eventDTO.MealType,
                    Payment = eventDTO.Payment,
                    Photo = eventDTO.Photo
                };

                _context.Add(newEvent);
                await _context.SaveChangesAsync();
                return new ActionResponse<Event>
                {
                    WasSuccess = true,
                    Result = newEvent
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Event>
                {
                    WasSuccess = false,
                    Message = "Ya existe un evento con el mismo nombre y la misma hora de inicio."
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Event>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public override async Task<ActionResponse<IEnumerable<Event>>> GetAsync(PaginationDTO pagination)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                query = query.Where(f => f.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Event>>
            {
                WasSuccess = true,
                Result = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var query = _context.Events.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                query = query.Where(f => f.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<Event>> UpdateAsync(EventDTO eventDTO)
        {
            try
            {
                var eventInfo = await _context.Events
                    .FirstOrDefaultAsync(e => e.Id == eventDTO.Id);

                if (eventInfo == null)
                {
                    return new ActionResponse<Event>
                    {
                        WasSuccess = false,
                        Message = "El evento no existe."
                    };
                }

                string? newImageUrl = eventInfo.Photo;
                if (!string.IsNullOrEmpty(eventDTO.Photo) && eventDTO.Photo != eventInfo.Photo)
                {
                    var photoEvent = Convert.FromBase64String(eventDTO.Photo);
                    newImageUrl = await _fileStorage.SaveFileAsync(photoEvent, ".jpg", "events");
                }

                eventInfo.Name = eventDTO.Name;
                eventInfo.Place = eventDTO.Place;
                eventInfo.StartDate = eventDTO.StartDate;
                eventInfo.EndDate = eventDTO.EndDate;
                eventInfo.PaymentDate = eventDTO.PaymentDate.Date;
                eventInfo.Description = eventDTO.Description;
                eventInfo.MealType = eventDTO.MealType;
                eventInfo.Payment = eventDTO.Payment;
                eventInfo.Photo = newImageUrl;

                _context.Update(eventInfo);
                await _context.SaveChangesAsync();
                return new ActionResponse<Event>
                {
                    WasSuccess = true,
                    Result = eventInfo
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Event>
                {
                    WasSuccess = false,
                    Message = "Ya existe un evento con el mismo nombre y la misma hora de inicio."
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Event>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}