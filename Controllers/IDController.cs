using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WeighForce.Data;

namespace Weighforce.Controllers
{
    [Route("ID")]
    // [Authorize()]
    public class IDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IDController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _context.ApplicationUser.ToList();
            var userRoles = _context.UserRoles.Select(r => new { UserId = r.UserId, Role = _context.Roles.Where(ro => ro.Id == r.RoleId).First().Name }).ToList();
            var userLocations = _context.OfficeUser.Select(o => new { UserId = o.UserId, Location = o.Office.Name }).ToList();
            return Ok(new { Users = users, UserRoles = userRoles, UserLocations = userLocations });
        }
    }
}
