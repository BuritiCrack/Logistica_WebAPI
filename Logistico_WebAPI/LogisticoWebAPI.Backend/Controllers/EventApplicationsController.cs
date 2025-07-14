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

        
        [HttpPost("apply")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostAsync([FromBody] ApplyToEventDTO applyToEventDTO)
        {
            var action = await _eventUsersUnitOfWork.ApplyToEventAsync(User.Identity!.Name!, applyToEventDTO);
            if (action.WassSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        
        [HttpDelete("cancel/{eventId:int}")]
        public async Task<IActionResult> CancelApplicationAsync(int eventId)
        {
            var action = await _eventUsersUnitOfWork.CancelApplicationAsync(User.Identity!.Name!, eventId);
            if (action.WassSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        
        [HttpGet("myapplications")]
        public override async Task<IActionResult> GetAsync()
        {
            var action = await _eventUsersUnitOfWork.GetUserApplicationsAsync(User.Identity!.Name!);
            if (action.WassSuccess)
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
            if (action.WassSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        
        //[HttpPut("{eventId:int}/user/{userId}/status")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> UpdateApplicationStatusAsync([FromBody] UpdateApplicationStatusDTO statusDto)
        //{
        //    try
        //    {
        //        // Buscar la aplicación específica
        //        var userApplications = await _eventUsersUnitOfWork.GetUserApplicationsAsync(User.Identity!.Name!);
        //        if (!userApplications.WassSuccess)
        //        {
        //            return BadRequest(new ActionResponses<UpdateApplicationStatusDTO>
        //            {
        //                WassSuccess = false,
        //                Message = userApplications.Message
        //            });
        //        }

        //        var application = userApplications.Result!.FirstOrDefault(a => a.EventId == statusDto.ApplicationId);
        //        if (application == null)
        //        {
        //            return NotFound(new ActionResponses<UpdateApplicationStatusDTO>
        //            {
        //                WassSuccess = false,
        //                Message = "Aplicación no encontrada."
        //            });
        //        }

        //        // Actualizar el estado usando el UnitOfWork
        //        var result = await _eventUsersUnitOfWork.UpdateApplicationStatusAsync(User.Identity!.Name!, statusDto);
        //        if (!result.WassSuccess)
        //        {
        //            return BadRequest(new ActionResponses<UpdateApplicationStatusDTO>
        //            {
        //                WassSuccess = false,
        //                Message = result.Message
        //            });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ActionResponses<UpdateApplicationStatusDTO>
        //        {
        //            WassSuccess = false,
        //            Message = $"Error interno del servidor: {ex.Message}"
        //        });
        //    }
        //}
    }
}