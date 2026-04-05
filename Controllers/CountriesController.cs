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
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesRepository _repo;
        private readonly IMapper _mapper;

        public CountriesController(ICountriesRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Countrys
        [HttpGet]
        public ActionResult<IEnumerable<CountryReadDto>> GetCountrys()
        {
            var items = _repo.GetAllCountries();
            return Ok(_mapper.Map<IEnumerable<CountryReadDto>>(items));
        }

        // GET: api/Countrys/5
        [HttpGet("{id}")]
        public ActionResult<CountryReadDto> GetCountry(long id)
        {
            var item = _repo.GetCountry(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CountryReadDto>(item));
        }
        [HttpPost]
        public IActionResult AddCountry(CountryReadDto country)
            => Ok(_mapper.Map<CountryReadDto>(_repo.AddCountry(_mapper.Map<Country>(country))));
        [HttpPut("{id}")]
        public IActionResult UpdateCountry(long id, CountryReadDto country)
            => Ok(_mapper.Map<CountryReadDto>(_repo.UpdateCountry(_mapper.Map<Country>(country))));

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
            => Ok(_repo.Delete(id, User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }
}
