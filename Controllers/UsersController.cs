using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        //TODO: Fix role checks
        // [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var usersRaw = _context.Users.ToList();
            var roles = _context.Roles.ToList();
            var locations = _context.Office.Where(l => l.IsDeleted == false).ToList();
            var userRoles = _context.UserRoles.ToList();
            var userLocations = _context.OfficeUser.ToList();
            var users = usersRaw.Select(x => new UserRoleDto
            {
                Email = x.Email,
                Name = x.Name,
                Roles = roles
                    .Where(y => userRoles.Any(z => z.UserId == x.Id && z.RoleId == y.Id))
                    .Select(y => y.NormalizedName),
                Locations = _mapper.Map<IEnumerable<OfficeReadDto>>(locations
                    .Where(y => userLocations.Any(z => z.UserId == x.Id && z.OfficeId == y.Id)))
            });
            return Ok(new UsersAndRolesDto
            {
                Users = users.ToList(),
                Locations = locations.Select(x => x.Name).ToList(),
                Roles = roles.Select(x => x.NormalizedName).ToList()
            });
        }
        [HttpGet("{email}")]
        public async Task<ActionResult> GetUserAsync(string email)
        {
            var userRaw = await _userManager.FindByEmailAsync(email);
            var roles = _context.Roles.ToList();
            var userRoles = _context.UserRoles.ToList();
            var locations = _context.Office.Include(l => l.Country).ToList();
            var userLocations = _context.OfficeUser.ToList();
            var user = new UserRoleDto
            {
                Email = userRaw.Email,
                Name = userRaw.Name,
                Roles = roles
                    .Where(y => userRoles.Any(z => z.UserId == userRaw.Id && z.RoleId == y.Id))
                    .Select(y => y.NormalizedName),
                Locations = _mapper.Map<IEnumerable<OfficeReadDto>>(locations
                    .Where(y => userLocations.Any(z => z.UserId == userRaw.Id && z.OfficeId == y.Id)))
            };
            return Ok(user);
        }

        //TODO: Fix role checks
        // [Authorize(Roles = "Admin")]
        [HttpGet("sync")]

        public ActionResult Sync()
        {
            var usersRaw = _context.Users.ToList();
            var roles = _context.Roles.ToList();
            var locations = _context.Office.ToList();
            var userRoles = _context.UserRoles.ToList();
            var userLocations = _context.OfficeUser.ToList();
            var users = usersRaw.Select(x => new UserSyncDto
            {
                Email = x.Email,
                Name = x.Name,
                PasswordHash = x.PasswordHash,
                SecurityStamp = x.SecurityStamp,
                ConcurrencyStamp = x.ConcurrencyStamp,
                Roles = roles
                    .Where(y => userRoles.Any(z => z.UserId == x.Id && z.RoleId == y.Id))
                    .Select(y => y.NormalizedName),
                Locations = _mapper.Map<IEnumerable<OfficeReadDto>>(locations
                    .Where(y => userLocations.Any(z => z.UserId == x.Id && z.OfficeId == y.Id)))
            });
            return Ok(users);
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public IActionResult AddUser(UserToggleDto dets)
        {
            if (_context.Users.Any(u => u.Email.ToLower() == dets.Email.ToLower()))
            {
                return NotFound();
            }
            var user = new ApplicationUser
            {
                UserName = dets.Email,
                Email = dets.Email,
                Name = dets.Name,
            };
            _userManager.CreateAsync(user, dets.Password).GetAwaiter().GetResult();
            return Ok(new UserRoleDto
            {
                Email = user.Email,
                Name = user.Name,
                Roles = new List<string>(),
                Locations = new OfficeReadDto[] { },
            });
        }

        [HttpPut("me")]
        [Authorize]
        public IActionResult UpdatePassword(UserToggleDto dets)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.ApplicationUser.Find(userId);
            var res = _userManager.ChangePasswordAsync(user, dets.CurrentPassword, dets.Password).GetAwaiter().GetResult();
            if (res.Succeeded)
                return Ok("Password Updated");
            else return Problem("Could not update password");
        }
        [HttpPut("{email}")]
        [Authorize]
        public async Task<IActionResult> ResetPasswordAsync(string email, PasswordDto password)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUserId = _userManager.GetUserId(User);
            var user = _context.ApplicationUser.FirstOrDefault(u => u.Email == email);
            var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole == null) return Forbid();
            var IsAdmin = _context.UserRoles
                .Any(ur => ur.UserId == userId && ur.RoleId == adminRole.Id);
            if (IsAdmin)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPassResult = await _userManager.ResetPasswordAsync(user, token, password.Password);
                if (resetPassResult.Succeeded)
                {
                    return Accepted();
                }
                else return BadRequest();
            }
            else return BadRequest();
        }

        [HttpPost("role")]
        // [Authorize(Roles = "Admin")]
        public IActionResult AddRole(UserToggleDto dets)
        {
            var user = _userManager.FindByEmailAsync(dets.Email).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, dets.Name).GetAwaiter().GetResult();
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("role")]
        // [Authorize(Roles = "Admin")]
        public IActionResult RemoveRole(UserToggleDto dets)
        {
            var user = _userManager.FindByEmailAsync(dets.Email).GetAwaiter().GetResult();
            _userManager.RemoveFromRoleAsync(user, dets.Name).GetAwaiter().GetResult();
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("mail")]
        // [Authorize(Roles = "Admin")]
        public IActionResult AddMail(UserAlertCreateDto dets)
        {
            var alert = _context.MailingList.FirstOrDefault(u => u.Email == dets.Email && u.Alert == dets.Alert && u.Office.Id == dets.Office.Id);
            var office = _context.Office.FirstOrDefault(u => u.Id == dets.Office.Id);
            if (alert != null)
            {
                return Ok();
            }
            _context.MailingList.Add(new MailingList
            {
                Email = dets.Email,
                Name = dets.Name,
                Alert = dets.Alert,
                Office = office
            });
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("mail")]
        // [Authorize(Roles = "Admin")]
        public IActionResult RemoveMail(UserAlertCreateDto dets)
        {
            var alert = _context.MailingList.FirstOrDefault(u => u.Email == dets.Email && u.Alert == dets.Name && u.Office.Id == dets.Office.Id);
            if (alert == null)
            {
                return Ok();
            }
            _context.MailingList.Remove(alert);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("mail")]
        // [Authorize(Roles = "Admin")]
        public IActionResult MailingList()
        {
            var alerts = _context.MailingList.Include(a => a.Office)
            .ToList()
            .GroupBy(x => x.Email)
            .Select(x =>
                x.Count() == 0 ? new UserAlertDto { }
                : new UserAlertDto
                {
                    Name = x.First().Name,
                    Email = x.First().Email,
                    Alerts = x
                      .Select(y => y.Alert + '-' + y.Office.Name),
                }
            );
            return Ok(alerts);
        }

        [HttpPost("location")]
        // [Authorize(Roles = "Admin")]
        public IActionResult AddLocation(UserAlertCreateDto dets)
        {
            var user = _userManager.FindByEmailAsync(dets.Email).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound("User " + dets.Email + " not found");
            }
            var office = _context.Office.FirstOrDefault(x => x.Name == dets.Name);
            if (office == null)
            {
                return NotFound("Office " + dets.Name + " not found");
            }
            var ou = new OfficeUser
            {
                UserId = user.Id,
                OfficeId = office.Id
            };
            if (_context.OfficeUser.Any(x => x.OfficeId == ou.OfficeId && x.UserId == ou.UserId))
            {
                return Ok("User " + dets.Email + " is already in Office " + dets.Name);
            }
            _context.OfficeUser.Add(ou);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("location")]
        // [Authorize(Roles = "Admin")]
        public IActionResult RemoveLocation(UserToggleDto dets)
        {
            var user = _userManager.FindByEmailAsync(dets.Email).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound("User " + dets.Email + " not found");
            }
            var office = _context.Office.FirstOrDefault(x => x.Name == dets.Name);
            if (office == null)
            {
                return NotFound("Office " + dets.Name + " not found");
            }
            var ou = new OfficeUser
            {
                UserId = user.Id,
                OfficeId = office.Id
            };
            if (!_context.OfficeUser.Any(x => x.OfficeId == ou.OfficeId && x.UserId == ou.UserId))
            {
                return Ok("User " + dets.Email + " not in Office " + dets.Name);
            }
            _context.OfficeUser.Remove(ou);
            _context.SaveChanges();
            return Ok();
        }
    }
}