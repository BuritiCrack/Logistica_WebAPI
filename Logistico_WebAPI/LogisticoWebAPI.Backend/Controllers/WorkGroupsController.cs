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
    public class WorkGroupsController : GenericController<WorkGroup>
    {
        private readonly IWorkGroupsUnitOfWork _workGroupsUnitOfWork;

        public WorkGroupsController(IGenericUnitOfWork<WorkGroup> unitOfWork, IWorkGroupsUnitOfWork workGroupsUnitOfWork) : base(unitOfWork)
        {
            _workGroupsUnitOfWork = workGroupsUnitOfWork;
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

        [HttpPost("full")]
        public async Task<IActionResult> PostAsync(WorkGroupDTO workGroupDTO)
        {
            var response = await _workGroupsUnitOfWork.AddAsync(workGroupDTO);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutAsync(WorkGroupDTO workGroupDTO)
        {
            var response = await _workGroupsUnitOfWork.UpdateAsync(workGroupDTO);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }
    }
}