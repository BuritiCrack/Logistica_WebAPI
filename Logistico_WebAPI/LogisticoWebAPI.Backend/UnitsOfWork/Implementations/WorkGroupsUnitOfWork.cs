using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;

namespace LogisticoWebAPI.Backend.UnitsOfWork.Implementations
{
    public class WorkGroupsUnitOfWork : GenericUnitOfWork<WorkGroup>, IWorkGroupsUnitOfWork
    {
        private readonly IWorkGroupsRepository _workGroupsRepository;
        public WorkGroupsUnitOfWork(IGenericRepository<WorkGroup> repository, IWorkGroupsRepository workGroupsRepository) : base(repository)
        {
            _workGroupsRepository = workGroupsRepository;
        }

        public override async Task<ActionResponse<WorkGroup>> GetAsync(int id)
            => await _workGroupsRepository.GetAsync(id);
        public override async Task<ActionResponse<IEnumerable<WorkGroup>>> GetAsync(PaginationDTO pagination) 
            => await _workGroupsRepository.GetAsync(pagination);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) 
            => await _workGroupsRepository.GetTotalPagesAsync(pagination);
        public async Task<ActionResponse<IEnumerable<WorkGroup>>> GetByEventAsync(int eventId) 
            => await _workGroupsRepository.GetByEventAsync(eventId);
        public async Task<ActionResponse<IEnumerable<WorkGroup>>> GetByCoordinatorAsync(string coordinatorId) 
            => await _workGroupsRepository.GetByCoordinatorAsync(coordinatorId);
        public async Task<ActionResponse<WorkGroupMember>> AddMemberAsync(WorkGroupMember member) 
            => await _workGroupsRepository.AddMemberAsync(member);
        public async Task<ActionResponse<bool>> RemoveMemberAsync(int memberId) 
            => await _workGroupsRepository.RemoveMemberAsync(memberId);
        public async Task<ActionResponse<IEnumerable<User>>> GetAvailableUsersForGroupAsync(int eventId, int workGroupId) 
            => await _workGroupsRepository.GetAvailableUsersForGroupAsync(eventId, workGroupId);

        public Task<ActionResponse<WorkGroup>> AddAsync(WorkGroup workGroup)
            => _workGroupsRepository.AddAsync(workGroup);

        public async Task<ActionResponse<WorkGroup>> UpdateAsync(WorkGroup workGroup)
            => await _workGroupsRepository.UpdateAsync(workGroup);

        async Task<ActionResponse<bool>> IWorkGroupsUnitOfWork.DeleteAsync(int id)
            => await _workGroupsRepository.DeleteAsync(id);

    }
}
