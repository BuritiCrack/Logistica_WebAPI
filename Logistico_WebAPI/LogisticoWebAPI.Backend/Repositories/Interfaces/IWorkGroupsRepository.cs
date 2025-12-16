using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.Repositories.Interfaces
{
    public interface IWorkGroupsRepository
    {
        Task<ActionResponse<WorkGroup>> GetAsync(int id);

        Task<ActionResponse<WorkGroup>> AddAsync(WorkGroupDTO workGroupDTO);

        Task<ActionResponse<WorkGroup>> UpdateAsync(WorkGroupDTO workGroupDTO);

        Task<ActionResponse<IEnumerable<WorkGroup>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<User>>> GetEventCoordinatorsAsync(int eventId);
    }
}