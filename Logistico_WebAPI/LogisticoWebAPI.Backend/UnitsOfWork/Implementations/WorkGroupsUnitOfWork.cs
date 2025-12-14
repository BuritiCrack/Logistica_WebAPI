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

        public WorkGroupsUnitOfWork(IGenericRepository<WorkGroup> repository, IWorkGroupsRepository
            workGroupsRepository) : base(repository)
        {
            _workGroupsRepository = workGroupsRepository;
        }

        public async Task<ActionResponse<WorkGroup>> AddAsync(WorkGroupDTO workGroupDTO, string email)
            => await _workGroupsRepository.AddAsync(workGroupDTO,email);

        public async Task<ActionResponse<WorkGroup>> UpdateAsync(WorkGroupDTO workGroupDTO)
            => await _workGroupsRepository.UpdateAsync(workGroupDTO);

        public override async Task<ActionResponse<IEnumerable<WorkGroup>>> GetAsync(PaginationDTO pagination)
            => await _workGroupsRepository.GetAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
            => await _workGroupsRepository.GetTotalPagesAsync(pagination);
    }
}