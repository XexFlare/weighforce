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
    public class TIsController : ControllerBase
    {
        private readonly ITIsRepository _repo;
        private readonly IMapper _mapper;

        public TIsController(ITIsRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/TIs
        [HttpGet]
        public ActionResult<IEnumerable<TIReadDto>> GetTIs()
        {
            var items = _repo.GetAllTIs();
            return Ok(_mapper.Map<IEnumerable<TIReadDto>>(items));
        }
        [HttpPost()]
        public ActionResult<TIReadDto> PostTI(long id, TIPutDto ti)
        {
            var item = _repo.PostTI(_mapper.Map<TransportInstruction>(ti));

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TIReadDto>(item));
        }
        [HttpGet("local")]
        public ActionResult<IEnumerable<TIReadDto>> GetLocalTIs()
        {
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            var items = _repo.GetLocalTIs(id);
            return Ok(_mapper.Map<IEnumerable<TIReadDto>>(items));
        }
        [HttpGet("inbound")]
        public ActionResult<IEnumerable<TIReadDto>> GetInboundTIs()
        {
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            var items = _repo.GetInboundTIs(id);
            return Ok(_mapper.Map<IEnumerable<TIReadDto>>(items));
        }

        // GET: api/TIs/5
        [HttpGet("{id}")]
        public ActionResult<TIReadDto> GetTI(long id)
        {
            var item = _repo.GetTI(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TIReadDto>(item));
        }
        [HttpPut("{id}")]
        public ActionResult<TIReadDto> UpdateTI(long id, TIPutDto ti)
        {
            var item = _repo.UpdateTI(id, _mapper.Map<TransportInstruction>(ti));

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TIReadDto>(item));
        }
        [HttpDelete("{id}/close")]
        public ActionResult<TIReadDto> CloseTI(long id)
        {
            var item = _repo.CloseTI(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TIReadDto>(item));
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
            => Ok(_repo.Delete(id, User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }
}
