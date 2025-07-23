using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticoWebAPI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventApplicationsController : GenericController<EventUser>
    {
        private readonly IEventUsersUnitOfWork _eventUsersUnitOfWork;

        public EventApplicationsController(IGenericUnitOfWork<EventUser> unitOfWork,
            IEventUsersUnitOfWork eventUsersUnitOfWork) : base(unitOfWork)
        {
            _eventUsersUnitOfWork = eventUsersUnitOfWork;
        }

        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetAsync(int eventId, [FromQuery] PaginationDTO pagination)
        {
            var action = await _eventUsersUnitOfWork.GetAsync(pagination, eventId);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages/{eventId:int}")]
        public async Task<IActionResult> GetPagesAsync(int eventId, [FromQuery] PaginationDTO pagination)
        {
            var action = await _eventUsersUnitOfWork.GetTotalPagesAsync(pagination, eventId);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost("apply/{eventId:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ApplyToEventAsync(int eventId)
        {
            var action = await _eventUsersUnitOfWork.ApplyToEventAsync(User.Identity!.Name!, eventId);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        
        [HttpDelete("cancel/{eventId:int}")]
        public async Task<IActionResult> CancelApplicationAsync(int eventId)
        {
            var action = await _eventUsersUnitOfWork.CancelApplicationAsync(User.Identity!.Name!, eventId);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        
        [HttpGet("myapplications")]
        public override async Task<IActionResult> GetAsync()
        {
            var action = await _eventUsersUnitOfWork.GetUserApplicationsAsync(User.Identity!.Name!);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        
        [HttpGet("applications/{eventId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAsync(int eventId)
        {
            var action = await _eventUsersUnitOfWork.GetEventApplicationsAsync(eventId);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        
        [HttpPut("updatestatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateApplicationStatusAsync(UpdateApplicationStatusDTO statusDto)
        {
            var action = await _eventUsersUnitOfWork.UpdateApplicationStatusAsync(User.Identity!.Name!, statusDto);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);

        }

        [HttpGet("statistics/{eventId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEventStatisticsAsync(int eventId)
        {
            var result = await _eventUsersUnitOfWork.GetEventStatisticsAsync(eventId);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result);
        }
    }
}