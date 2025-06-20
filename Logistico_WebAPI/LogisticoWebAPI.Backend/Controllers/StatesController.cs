using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticoWebAPI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatesController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(State state)
        {
            _context.States.Add(state);
            await _context.SaveChangesAsync();
            return Ok(state);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var states = await _context.States.ToListAsync();
            return Ok(states);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetByIdAsycn(int id)
        {
            var state = await _context.States.FirstOrDefaultAsync(x => x.Id == id);
            if (state == null)
            {
                return NotFound();
            }
            return Ok(state);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var state = await _context.States.FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound("El departamento no fue encontrado");
            }

            _context.States.Remove(state);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UptateAsync(State state)
        {
            _context.States.Update(state);
            await _context.SaveChangesAsync();
            return Ok(state);
        }
    }
}
