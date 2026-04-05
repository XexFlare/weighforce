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
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _repo;
        private readonly IMapper _mapper;

        public ProductsController(IProductsRepository context, IMapper mapper)
        {
            _repo = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<ProductReadDto>> GetProducts()
        {
            var items = _repo.GetAllProducts();
            return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(items));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<ProductReadDto> GetProduct(long id)
        {
            var item = _repo.GetProduct(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductReadDto>(item));
        }

        [HttpPost]
        public IActionResult AddProduct(ProductReadDto product)
        {
            try
            {
                var res = _repo.AddProduct(_mapper.Map<Product>(product));
                return Ok(res);
            }
            catch (System.Exception e)
            {
                return ValidationProblem(e.Message);
            }
        }
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(long id, ProductReadDto product)
        {
            try
            {
                var res = _repo.UpdateProduct(id, _mapper.Map<Product>(product));
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
