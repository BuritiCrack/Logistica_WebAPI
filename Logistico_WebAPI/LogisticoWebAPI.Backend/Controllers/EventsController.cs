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

    public class EventsController : GenericController<Event>
    {
        private readonly IEventsUnitOfWork _eventsUnitOfWork;

        public EventsController(IGenericUnitOfWork<Event> unitOfWork, IEventsUnitOfWork eventsUnitOfWork) : base(unitOfWork)
        {
            _eventsUnitOfWork = eventsUnitOfWork;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
        {
            var action = await _eventsUnitOfWork.GetAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _eventsUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost("full")]
        public async Task<IActionResult> PostAsync(EventDTO eventDTO)
        {
            var action = await _eventsUnitOfWork.AddAsync(eventDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutAsync(EventDTO eventDTO)
        {
            var action = await _eventsUnitOfWork.UpdateAsync(eventDTO);
            if(action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }


    }
}
