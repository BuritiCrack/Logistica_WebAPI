using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Repositories.Implementations
{
    public class WorkGroupsRepository : GenericRepository<WorkGroup>, IWorkGroupsRepository
    {
        private readonly DataContext _context;

        public WorkGroupsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<WorkGroup>> GetAsync(int id)
        {
            var workgroup = await _context.WorkGroups
                .Include(wg => wg.Event)
                .Include(wg => wg.Coordinator)
                .Include(wg => wg.Members!)
                    .ThenInclude(m => m.User)
                    .FirstOrDefaultAsync(wg => wg.Id == id);

            if (workgroup == null)
            {
                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = false,
                    Message = "Grupo de trabajo no encontrado"
                };
            }

            return new ActionResponse<WorkGroup>
            {
                WasSuccess = true,
                Result = workgroup
            };
        }

        public async Task<ActionResponse<WorkGroup>> AddAsync(WorkGroupDTO workGroupDTO)
        {
            try
            {
                var workGroup = new WorkGroup
                {
                    Name = workGroupDTO.Name,
                    Description = workGroupDTO.Description,
                    CoordinatorId = workGroupDTO.CoordinatorId,
                    EventId = workGroupDTO.EventId
                };

                _context.WorkGroups.Add(workGroup);
                await _context.SaveChangesAsync();

                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = true,
                    Result = workGroup
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = false,
                    Message = "Ya existe un grupo de trabajo con el mismo nombre para este evento."
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<WorkGroup>> UpdateAsync(WorkGroupDTO workGroupDTO)
        {
            try
            {
                var existingWorkGroup = await _context.WorkGroups
                    .FirstOrDefaultAsync(wg => wg.Id == workGroupDTO.Id);

                if (existingWorkGroup == null)
                {
                    return new ActionResponse<WorkGroup>
                    {
                        WasSuccess = false,
                        Message = "Grupo de trabajo no encontrado"
                    };
                }

                existingWorkGroup.Name = workGroupDTO.Name;
                existingWorkGroup.Description = workGroupDTO.Description;

                _context.WorkGroups.Update(existingWorkGroup);
                await _context.SaveChangesAsync();

                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = true,
                    Result = existingWorkGroup
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = false,
                    Message = "Ya existe un grupo de trabajo con el mismo nombre para este evento."
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<WorkGroup>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public override async Task<ActionResponse<IEnumerable<WorkGroup>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.WorkGroups
                .Include(wg => wg.Event)
                .Include(wg => wg.Coordinator)
                .Include(wg => wg.Members)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<WorkGroup>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.WorkGroups.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }
    }
}