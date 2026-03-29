using Microsoft.AspNetCore.Mvc;
using rec_be.Data;
using Microsoft.EntityFrameworkCore;

namespace rec_be.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RACPostgreSQLDbContext _context;

        public RoomController(RACPostgreSQLDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guests = await _context.Rooms.ToListAsync();
            return Ok(guests);
        }
    }
}