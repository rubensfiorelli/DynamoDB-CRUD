using Amazon.DynamoDBv2.DataModel;
using DynamoDB_Test.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDB_Test.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public ProductsController(IDynamoDBContext context) => _context = context;

        [HttpGet("{id}/{barcode}")]
        public async Task<IActionResult> Get(string id, string barcode)
        {
            var product = await _context.LoadAsync<Product>(id, barcode);

            if(product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.ScanAsync<Product>(default).GetRemainingAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product request)
        {
            var product = await _context.LoadAsync<Product>(request.Id, request.Barcode);

            if(product is not null) return BadRequest($"Product with ID {request.Id} and Barcode {request.Barcode} exists");
            await _context.SaveAsync(request);

            return Ok(request);

        }

        [HttpDelete("{id}/{barcode}")]
        public async Task<IActionResult> Delete(string id, string barcode)
        {
            var product = await _context.LoadAsync<Product>(id, barcode);

            if (product is null)
                return NotFound();

            await _context.DeleteAsync(product);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Product request)
        {
            var product = await _context.LoadAsync<Product>(request.Id, request.Barcode);

            if (product is null)
                return NotFound();

            await _context.SaveAsync(request);

            return Ok(request);
        }
    }
}
