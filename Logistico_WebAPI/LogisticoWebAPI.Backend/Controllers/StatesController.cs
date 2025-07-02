using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class StatesController : GenericController<State>
    {
        private readonly IStatesUnitOfWork _statesUnitOfWork;

        public StatesController(IGenericUnitOfWork<State> unitOfWork, IStatesUnitOfWork statesUnitOfWork) : base(unitOfWork)
        {
            _statesUnitOfWork = statesUnitOfWork;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync()
        {
            var action = await _statesUnitOfWork.GetAllAsync();
            if (action.WassSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            var action = await _statesUnitOfWork.GetAsync(id);
            if (action.WassSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
    }
}
