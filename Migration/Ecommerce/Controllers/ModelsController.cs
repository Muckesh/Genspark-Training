using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;
        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var models = await _modelService.GetAllModels();
                return Ok(models);
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
                var model = await _modelService.GetModelById(id);
                return Ok(model);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ModelRequestDto requestDto)
        {
            try
            {
                var model = await _modelService.GetModelById(id);
                model = await _modelService.UpdateModel(id, requestDto);
                return Ok(model);
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
                var model = await _modelService.GetModelById(id);
                model = await _modelService.DeleteModel(id);
                return Ok(model);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ModelRequestDto requestDto)
        {
            try
            {
                var model = await _modelService.CreateModel(requestDto);
                return Ok(model);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}