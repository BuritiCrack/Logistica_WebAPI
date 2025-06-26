using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LogisticoWebAPI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : GenericController<Event>
    {
        public EventsController(IGenericUnitOfWork<Event> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
