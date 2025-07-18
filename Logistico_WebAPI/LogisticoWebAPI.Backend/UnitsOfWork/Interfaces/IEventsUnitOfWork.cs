using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface IEventsUnitOfWork
    {
        Task<ActionResponse<Event>> GetAsync(int id);

        Task<ActionResponse<Event>> AddAsync(EventDTO eventDTO);

        Task<ActionResponse<Event>> UpdateAsync(EventDTO eventDTO);
        Task<ActionResponse<IEnumerable<Event>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}