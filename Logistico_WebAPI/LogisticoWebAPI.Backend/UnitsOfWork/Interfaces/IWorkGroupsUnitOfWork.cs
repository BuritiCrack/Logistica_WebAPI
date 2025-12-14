using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Interfaces
{
    public interface IWorkGroupsUnitOfWork
    {
        Task<ActionResponse<WorkGroup>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<WorkGroup>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<WorkGroup>>> GetByEventAsync(int eventId);
        Task<ActionResponse<IEnumerable<WorkGroup>>> GetByCoordinatorAsync(string coordinatorId);
        Task<ActionResponse<WorkGroup>> AddAsync(WorkGroup workGroup);
        Task<ActionResponse<WorkGroup>> UpdateAsync(WorkGroup workGroup);
        Task<ActionResponse<bool>> DeleteAsync(int id);
        Task<ActionResponse<WorkGroupMember>> AddMemberAsync(WorkGroupMember member);
        Task<ActionResponse<bool>> RemoveMemberAsync(int memberId);
        Task<ActionResponse<IEnumerable<User>>> GetAvailableUsersForGroupAsync(int eventId, int workGroupId);
    }
}
