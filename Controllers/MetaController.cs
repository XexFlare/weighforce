using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeighForce.Data.Repositories;
using AutoMapper;

namespace WeighForce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MetaController : ControllerBase
    {
        private readonly IMetaRepository _repo;
        private readonly IMapper _mapper;

        public MetaController(IMetaRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Meta/5
        [HttpGet("{Name}")]
        public ActionResult<Meta> GetMeta(string Name)
            => Ok(_repo.GetMeta(Name));

        [HttpPost()]
        public IActionResult SetMeta(Meta meta)
            => Ok(_repo.SetMeta(meta));

        [HttpDelete("{id}")]
        public IActionResult Delete(string name)
            => Ok(_repo.DeleteMeta(name));
    }
}
