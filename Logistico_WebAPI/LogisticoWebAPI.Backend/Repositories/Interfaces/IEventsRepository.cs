using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IEventsRepository
    {
        Task<ActionResponse<IEnumerable<Event>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}