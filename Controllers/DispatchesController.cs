using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeighForce.Data.Repositories;
using WeighForce.Dtos;
using AutoMapper;
using WeighForce.Data;
using WeighForce.Models;
using System.Security.Claims;
using WeighForce.Filters;
using WeighForce.Wrappers;
using System;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DispatchesController : ControllerBase
    {
        private readonly IDispatchesRepository _repo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public DispatchesController(IDispatchesRepository repo, IMapper mapper, ApplicationDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Dispatches
        [HttpGet]
        public ActionResult<IEnumerable<DispatchReadDto>> GetAll([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetPendingDispatches(id, validFilter, _mapper);
            return Ok(new PagedResponse<IEnumerable<DispatchFirstWeightDto>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }

        [HttpGet("speed")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetSpeedStuff([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetAllWagons(id, validFilter, _mapper);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }

        [HttpGet("to-send")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetWagonsToSend([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            var items = _repo.GetToSend(_mapper);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }

        [HttpGet("pending")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetPendingWagons([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetPendingWagons(id, validFilter, _mapper);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }
        // GET: api/Dispatches
        [HttpGet("temp")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetTempDispatches([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetTempDispatches(id, validFilter);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }
        // GET: api/Dispatches
        [HttpGet("clients")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetClientDispatches([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetClientDispatches(id, validFilter);
            return Ok(_mapper.Map<IEnumerable<DispatchReadDto>>(items));
        }
        // GET: api/Dispatches
        [HttpGet("chart")]
        public ActionResult<IEnumerable<DayInfo>> GetChart([FromQuery] string type)
        {
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var days = _repo.GetChart(id, 7, type);
            return Ok(days);
        }
        [HttpGet("dash")]
        public ActionResult<Dashboard> GetDashboard([FromQuery] string type,[FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var days = _repo.GetDashboard(id, 7, from, to);
            return Ok(days);
        }
        [HttpGet("report")]
        public ActionResult<DispatchReport> GetDispatchReport()
        {
            var days = _repo.GetDispatchReport();
            return Ok(days);
        }
        [HttpGet("Processed")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetProcessed([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetProcessedDispatches(id, validFilter, _mapper);
            return Ok(new PagedResponse<IEnumerable<DispatchFirstWeightDto>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }
        [HttpGet("Overweights")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetOverweights([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetOverweights(id, validFilter);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }
        [HttpGet("Underweights")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetUnderweights([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To);
            int id = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out id);
            if (!v)
                return Unauthorized();
            var items = _repo.GetUnderweights(id, validFilter);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetPendingDispatches(long id, [FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.Page, filter.Size, filter.From, filter.To, filter.Type);
            var items = _repo.GetPendingDispatches(id, validFilter, _mapper);
            return Ok(new PagedResponse<IEnumerable<DispatchFirstWeightDto>, IEnumerable<DispatchReadDto>>(items, validFilter, _mapper));
        }
        [HttpGet("{id}/related")]
        public ActionResult<IEnumerable<DispatchReadDto>> GetCloseDispatches(long id)
        {
            var filter = new PaginationFilter
            {
                Size = -1,
                Page = 1
            };
            int officeId = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out officeId);
            var items = _repo.GetDispatchesDuring(officeId, id);
            return Ok(new PagedResponse<IEnumerable<Dispatch>, IEnumerable<DispatchReadDto>>(items, filter, _mapper));
        }

        [HttpPost("{id}/print")]
        public IActionResult Print(long id)
        {
            if (_repo.Print(id))
                return Ok();
            else return NotFound();
        }
        [HttpPost("{id}/assign")]
        public ActionResult<DispatchReadDto> Reassign(long id, ReassignPost post)
        {
            var item = _repo.ReassignDispatch(id, post.product, post.numberPlate);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DispatchReadDto>(item));
        }
        [HttpPost("{id}/discard")]
        public ActionResult<DispatchReadDto> Discard(long id)
        {
            var item = _repo.DiscardDispatch(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DispatchReadDto>(item));
        }

        [HttpPost]
        [Route("clients/{id}")]
        public ActionResult<DispatchReadDto> PostClientWeight(long id, ClientWeight weight)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var item = _repo.PostClientWeight(id, weight.Tare, weight.Gross, userId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DispatchReadDto>(item));
        }
        [HttpPost("{id}")]
        public ActionResult<DispatchReadDto> PostWeight(long id, ScaleWeight details)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var item = _repo.PostWeight(id, details, userId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DispatchReadDto>(item));
        }
        
        [HttpPost("{id}/init")]
        public ActionResult<DispatchReadDto> UpdateWeight(long id, ClientWeight weight)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var item = _repo.PostInitialWeight(id, weight.Tare, weight.Gross, userId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DispatchReadDto>(item));
        }

        [HttpGet("search/{query}")]
        public ActionResult<IEnumerable<DispatchReadDto>> Search(string query)
        {
            int officeId = 0;
            bool v = int.TryParse(Request.Headers["OfficeId"], out officeId);
            var items = _repo.Search(officeId, query);
            return Ok(_mapper.Map<IEnumerable<DispatchReadDto>>(items));
        }
    }

}
