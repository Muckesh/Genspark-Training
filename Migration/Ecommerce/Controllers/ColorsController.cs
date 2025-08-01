using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;
        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var colors = await _colorService.GetAllColors();
                return Ok(colors);
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
                var color = await _colorService.GetColorById(id);
                return Ok(color);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ColorRequestDto requestDto)
        {
            try
            {
                var color = await _colorService.GetColorById(id);
                color = await _colorService.UpdateColor(id, requestDto);
                return Ok(color);
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
                var color = await _colorService.GetColorById(id);
                color = await _colorService.DeleteColor(id);
                return Ok(color);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ColorRequestDto requestDto)
        {
            try
            {
                var color = await _colorService.CreateColor(requestDto);
                return Ok(color);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}