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
    public class TransportersController : ControllerBase
    {
        private readonly ITransportersRepository _repo;
        private readonly IMapper _mapper;

        public TransportersController(ITransportersRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/transporters
        [HttpGet]
        public ActionResult<IEnumerable<TransporterDto>> GetTransporters()
        {
            var items = _repo.GetAllTransporters();
            return Ok(_mapper.Map<IEnumerable<TransporterDto>>(items));
        }
        [HttpPost()]
        public ActionResult<TransporterDto> PostTI(long id, TransporterPutDto transporter)
        {
            var item = _repo.PostTransporter(_mapper.Map<Transporter>(transporter));
            return Ok(_mapper.Map<TransporterDto>(item));
        }

        // GET: api/transporters/5
        [HttpGet("{id}")]
        public ActionResult<TransporterDto> GetTransporter(long id)
        {
            var item = _repo.GetTransporter(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TransporterDto>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<TransporterDto> UpdateTransporter(long id, TransporterPutDto transporter)
        {
            var item = _repo.UpdateTransporter(id, _mapper.Map<Transporter>(transporter));

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TransporterDto>(item));
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
            => Ok(_repo.Delete(id, User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }
}
