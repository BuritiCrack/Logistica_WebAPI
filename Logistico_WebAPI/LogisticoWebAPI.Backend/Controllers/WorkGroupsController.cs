using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace LogisticoWebAPI.Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkGroupsController : GenericController<WorkGroup>
    {
        private readonly IWorkGroupsUnitOfWork _workGroupsUnitOfWork;
        public WorkGroupsController(IGenericUnitOfWork<WorkGroup> unitOfWork, IWorkGroupsUnitOfWork workGroupsUnitOfWork) : base(unitOfWork)
        {
            _workGroupsUnitOfWork = workGroupsUnitOfWork;
        }

        [HttpGet("full/{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _workGroupsUnitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _workGroupsUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _workGroupsUnitOfWork.GetTotalPagesAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("byEvent/{eventId}")]
        public async Task<IActionResult> GetByEventAsync(int eventId)
        {
            var response = await _workGroupsUnitOfWork.GetByEventAsync(eventId);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("byCoordinator/{coordinatorId}")]
        public async Task<IActionResult> GetByCoordinatorAsync(string coordinatorId)
        {
            var response = await _workGroupsUnitOfWork.GetByCoordinatorAsync(coordinatorId);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("availableUsers/{eventId}/{workGroupId}")]
        public async Task<IActionResult> GetAvailableUsersAsync(int eventId, int workGroupId)
        {
            var response = await _workGroupsUnitOfWork.GetAvailableUsersForGroupAsync(eventId, workGroupId);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpPost("addMember")]
        public async Task<IActionResult> AddMemberAsync(WorkGroupMember member)
        {
            var response = await _workGroupsUnitOfWork.AddMemberAsync(member);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }

        [HttpDelete("removeMember/{memberId}")]
        public async Task<IActionResult> RemoveMemberAsync(int memberId)
        {
            var response = await _workGroupsUnitOfWork.RemoveMemberAsync(memberId);
            if (response.WasSuccess)
            {
                return NoContent();
            }
            return NotFound(response.Message);
        }
    }
}
