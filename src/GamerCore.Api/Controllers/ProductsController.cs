using GamerCore.Api.Models;
using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
        {
            var products = await _repository.GetProducts().ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Categories = p.ProductCategories.Select(pc => new CategoryDto
                {
                    CategoryId = pc.Category.CategoryId,
                    Name = pc.Category.Name
                }).ToList()
            }).ToList();

            return Ok(productDtos);
        }
    }
}