using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryParamsDto paramsDto)
        {
            try
            {
                var products = await _productService.GetAllProducts(paramsDto);
                return Ok(products);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                return Ok(product);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateRequestDto requestDto)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                product = await _productService.UpdateProduct(id, requestDto);
                return Ok(product);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                product = await _productService.DeleteProduct(id);
                return Ok(product);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ProductRequestDto requestDto)
        {
            try
            {
                var product = await _productService.CreateProduct(requestDto);
                return Ok(product);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

    }
}