using AutoMapper;
using BillApplication.Dto;
using BillApplication.Interface;
using BillApplication.Models;
using BillApplication.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BillApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProizvodController : Controller
    {
        private readonly IProizvodRepository _productRepository;
        private readonly IMapper _mapper;

        public ProizvodController(IProizvodRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Proizvod>))]

        public IActionResult GetProducts()
        {
            var products = _mapper.Map<List<ProizvodDto>>(_productRepository.GetProducts());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(products);

        }

        [HttpGet("{productId}")]
        [ProducesResponseType(200, Type = typeof(Proizvod))]
        [ProducesResponseType(400)]

        public IActionResult GetProduct(int productId)

        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();
            var product = _mapper.Map<ProizvodDto>(_productRepository.GetProduct(productId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(product);

        }
        [HttpPost("CreateProduct")]
        public IActionResult CreateProduct([FromBody] ProizvodDto proizvodDto)
        {
            if (proizvodDto == null || string.IsNullOrEmpty(proizvodDto.Name) || proizvodDto.Price <= 0)
            {
                return BadRequest(new { message = "Invalid product data. Ensure Name, Price, and Active are provided." });
            }

            // Pozivanje metode za umetanje proizvoda
            _productRepository.CreateProduct(proizvodDto.Name, proizvodDto.Price, proizvodDto.Active);

            return Ok(new { message = "Product created successfully." });
        }
        [HttpPut("UpdateProduct/{productId}")]
        public IActionResult UpdateProduct(int productId, [FromBody] ProizvodDto proizvodDto)
        {
            if (proizvodDto == null || proizvodDto.ProductID != productId)
            {
                return BadRequest("Invalid product data.");
            }

            // Pozivanje metode za ažuriranje proizvoda
            _productRepository.UpdateProduct(productId, proizvodDto.Name, proizvodDto.Price, proizvodDto.Active);

            return Ok("Product updated successfully.");
        }
        [HttpGet("GetAllProduct")]
        public async Task<ActionResult<IReadOnlyList<Proizvod>>>GetAllProduct(string?name, string? sort)
        {
            return Ok(await _productRepository.GetAll(name, sort)); 
        }
    }
}
