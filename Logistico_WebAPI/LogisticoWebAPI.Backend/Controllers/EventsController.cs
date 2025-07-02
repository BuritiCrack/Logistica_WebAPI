using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
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
        public EventsController(IGenericUnitOfWork<Event> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
