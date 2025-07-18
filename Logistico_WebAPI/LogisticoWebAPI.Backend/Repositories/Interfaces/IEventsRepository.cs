using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IEventsRepository
    {
        Task<ActionResponse<Event>> GetAsync(int id); //TODO:Probar implementacion a ver si es mas facil por este lado

        Task<ActionResponse<Event>> AddAsync(EventDTO eventDTO);

        Task<ActionResponse<Event>> UpdateAsync(EventDTO eventDTO);

        Task<ActionResponse<IEnumerable<Event>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}