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
    public class ContractsController : ControllerBase
    {
        private readonly IContractsRepository _repo;
        private readonly IMapper _mapper;

        public ContractsController(IContractsRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Contracts
        [HttpGet]
        public ActionResult<IEnumerable<ContractReadDto>> GetContracts()
        {
            var items = _repo.GetAllContracts();
            return Ok(_mapper.Map<IEnumerable<ContractReadDto>>(items));
        }
        [HttpPost()]
        public ActionResult<ContractReadDto> PostContract(ContractWriteDto contract)
        {
            var item = _repo.CreateContract(_mapper.Map<Contract>(contract));

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ContractReadDto>(item));
        }

        // GET: api/Contracts/5
        [HttpGet("{id}")]
        public ActionResult<ContractReadDto> GetContract(long id)
        {
            var item = _repo.GetContract(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ContractReadDto>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<ContractReadDto> UpdateContract(long id, ContractReadDto contract)
        {
            var item = _repo.UpdateContract(id, _mapper.Map<Contract>(contract));

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ContractReadDto>(item));
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
            => Ok(_repo.Delete(id, User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }
}
