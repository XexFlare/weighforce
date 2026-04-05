using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeighForce.Data.Repositories;
using WeighForce.Dtos;
using AutoMapper;
using WeighForce.Models;
using System.Security.Claims;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitRepository _repo;
        private readonly IMapper _mapper;

        public UnitsController(IUnitRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Units
        [HttpGet]
        public ActionResult<IEnumerable<UnitReadDto>> GetUnits()
        {
            var items = _repo.GetAllUnits();
            return Ok(_mapper.Map<IEnumerable<UnitReadDto>>(items));
        }

        [HttpPost]
        public IActionResult AddUnit(UnitReadDto Unit)
        {
            try
            {
                var res = _repo.AddUnit(_mapper.Map<Unit>(Unit));
                return Ok(res);
            }
            catch (System.Exception e)
            {
                return ValidationProblem(e.Message);
            }
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUnit(long id, UnitReadDto Unit)
        {
            try
            {
                var res = _repo.UpdateUnit(id, _mapper.Map<Unit>(Unit));
                return Ok(res);
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
