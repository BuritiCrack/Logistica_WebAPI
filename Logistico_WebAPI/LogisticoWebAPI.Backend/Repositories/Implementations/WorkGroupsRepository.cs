using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
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

        public Task<ActionResponse<WorkGroup>> AddAsync(WorkGroup workGroup)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResponse<WorkGroupMember>> AddMemberAsync(WorkGroupMember member)
        {
            // Verificar que el grupo existe
            var workGroup = await _context.WorkGroups
                .Include(wg => wg.Event)
                .FirstOrDefaultAsync(wg => wg.Id == member.WorkGroupId);

            if (workGroup == null)
            {
                return new ActionResponse<WorkGroupMember>
                {
                    WasSuccess = false,
                    Message = "Grupo de trabajo no encontrado"
                };
            }

            // Verificar que el usuario está registrado en el evento
            var eventUser = await _context.EventUsers
                .FirstOrDefaultAsync(eu => eu.EventId == workGroup.EventId
                                        && eu.UserId == member.UserId
                                        && eu.Status == ApplicationStatus.Accepted);

            if (eventUser == null)
            {
                return new ActionResponse<WorkGroupMember>
                {
                    WasSuccess = false,
                    Message = "El usuario no está aprobado en este evento"
                };
            }

            // Verificar que no está ya en el grupo
            var exists = await _context.WorkGroupMembers
                .AnyAsync(wgm => wgm.WorkGroupId == member.WorkGroupId
                              && wgm.UserId == member.UserId);

            if (exists)
            {
                return new ActionResponse<WorkGroupMember>
                {
                    WasSuccess = false,
                    Message = "El usuario ya es miembro de este grupo"
                };
            }

            _context.WorkGroupMembers.Add(member);
            await _context.SaveChangesAsync();

            return new ActionResponse<WorkGroupMember>
            {
                WasSuccess = true,
                Result = member
            };
        }

        public async Task<ActionResponse<IEnumerable<User>>> GetAvailableUsersForGroupAsync(int eventId, int workGroupId)
        {
            // Usuarios aprobados en el evento
            var eventUserIds = await _context.EventUsers
                .Where(eu => eu.EventId == eventId && eu.Status == ApplicationStatus.Accepted)
                .Select(eu => eu.UserId)
                .ToListAsync();

            // Usuarios ya asignados a este grupo
            var assignedUserIds = await _context.WorkGroupMembers
                .Where(wgm => wgm.WorkGroupId == workGroupId)
                .Select(wgm => wgm.UserId)
                .ToListAsync();

            // Usuarios disponibles (en el evento pero no en este grupo)
            var availableUsers = await _context.Users
                .Where(u => eventUserIds.Contains(u.Id) && !assignedUserIds.Contains(u.Id))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            return new ActionResponse<IEnumerable<User>>
            {
                WasSuccess = true,
                Result = availableUsers
            };
        }

        public async Task<ActionResponse<IEnumerable<WorkGroup>>> GetByCoordinatorAsync(string coordinatorId)
        {
            var workGroups = await _context.WorkGroups
                .Include(wg => wg.Event)
                .Include(wg => wg.Members!)
                    .ThenInclude(m => m.User)
                .Where(wg => wg.CoordinatorId == coordinatorId)
                .OrderBy(wg => wg.Event!.StartDate)
                .ThenBy(wg => wg.Name)
                .ToListAsync();

            return new ActionResponse<IEnumerable<WorkGroup>>
            {
                WasSuccess = true,
                Result = workGroups
            };
        }

        public async Task<ActionResponse<IEnumerable<WorkGroup>>> GetByEventAsync(int eventId)
        {
            var workGroups = await _context.WorkGroups
                .Include(wg => wg.Coordinator)
                .Include(wg => wg.Members!)
                    .ThenInclude(m => m.User)
                .Where(wg => wg.EventId == eventId)
                .OrderBy(wg => wg.Name)
                .ToListAsync();

            return new ActionResponse<IEnumerable<WorkGroup>>
            {
                WasSuccess = true,
                Result = workGroups
            };
        }

        public async Task<ActionResponse<bool>> RemoveMemberAsync(int memberId)
        {
            var member = await _context.WorkGroupMembers.FindAsync(memberId);

            if (member == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Miembro no encontrado"
                };
            }

            _context.WorkGroupMembers.Remove(member);
            await _context.SaveChangesAsync();

            return new ActionResponse<bool>
            {
                WasSuccess = true,
                Result = true
            };
        }
        
        
        public Task<ActionResponse<WorkGroup>> UpdateAsync(WorkGroup workGroup)
        {
            throw new NotImplementedException();
        }

        Task<ActionResponse<bool>> IWorkGroupsRepository.DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}