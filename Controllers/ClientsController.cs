using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeighForce.Data.Repositories;
using WeighForce.Dtos;
using AutoMapper;
using System.Security.Claims;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsRepository _repo;
        private readonly IMapper _mapper;

        public ClientsController(IClientsRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Clients
        [HttpGet]
        public ActionResult<IEnumerable<ClientReadDto>> GetClients()
        {
            var items = _repo.GetAllClients();
            return Ok(_mapper.Map<IEnumerable<ClientReadDto>>(items));
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public ActionResult<ClientReadDto> GetClient(long id)
        {
            var item = _repo.GetClient(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ClientReadDto>(item));
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
            => Ok(_repo.Delete(id, User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }
}
