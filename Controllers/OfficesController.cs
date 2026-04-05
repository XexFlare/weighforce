using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WeighForce.Models;
using Microsoft.AspNetCore.Authorization;
using WeighForce.Data.Repositories;
using WeighForce.Dtos;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using WeighForce.Services;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OfficesController : ControllerBase
    {
        private readonly IOfficesRepository _repo;
        private readonly IMapper _mapper;

        public OfficesController(IOfficesRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Offices
        [HttpGet]
        public ActionResult<IEnumerable<OfficeReadDto>> GetOffices()
        {
            var items = _repo.GetAllOffices();
            return Ok(_mapper.Map<IEnumerable<OfficeReadDto>>(items));
        }
        [HttpGet("user")]
        public ActionResult<IEnumerable<OfficeReadDto>> GetUserOffices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var items = _repo.GetUserOffices(userId);
            return Ok(_mapper.Map<IEnumerable<OfficeReadDto>>(items));
        }

        // GET: api/Offices/5
        [HttpGet("{id}")]
        public ActionResult<BranchedOfficeDto> GetOffice(long id)
        {
            var item = _repo.GetOffice(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BranchedOfficeDto>(item));
        }
        [HttpPost]
        public async Task<IActionResult> AddOffice(IFormCollection data)
        {
            try
            {
                var office = JsonSerializer.Deserialize<OfficePostDto>(data["details"]);
                foreach (var file in data.Files){
                    string path = await FileUploadService.WriteFile(file, "wwwroot/logos", true);
                    office.Logo = "/logos/" + path;
                }
                var res = _repo.AddOffice(_mapper.Map<Office>(office));
                return Ok(_mapper.Map<OfficeReadDto>(res));
            }
            catch (System.Exception e)
            {
                return ValidationProblem(e.Message);
            }
        }
        
        [HttpPost("branch")]
        public IActionResult AddBranch(BranchDto branch)
        {
            try
            {
                var res = _repo.AddBranch(_mapper.Map<Branch>(branch));
                return Ok(_mapper.Map<BranchDto>(res));
            }
            catch (System.Exception e)
            {
                return ValidationProblem(e.Message);
            }
        }
        [HttpPut("branch/{id}")]
        public IActionResult UpdateBranch(long id, BranchDto branch)
        {
            try
            {
                var res = _repo.UpdateBranch(id, _mapper.Map<Branch>(branch));
                return Ok(_mapper.Map<BranchDto>(res));
            }
            catch (System.Exception e)
            {
                return ValidationProblem(e.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffice(long id, IFormCollection data)
        {
            try
            {
                long size = data.Files.Sum(f => f.Length);
                var office = JsonSerializer.Deserialize<OfficePostDto>(data["details"]);
                foreach (var file in data.Files){
                    string path = await FileUploadService.WriteFile(file, "wwwroot/logos", true);
                    office.Logo = "/logos/" + path;
                }
                var res = _repo.UpdateOffice(id, _mapper.Map<Office>(office));
                return Ok(_mapper.Map<OfficeReadDto>(res));
            }
            catch (System.Exception e)
            {
                return ValidationProblem(e.Message);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
            => Ok(_repo.Delete(id, User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }
}
