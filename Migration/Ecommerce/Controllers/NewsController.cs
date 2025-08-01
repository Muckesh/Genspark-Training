using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var news = await _newsService.GetAllNews();
                return Ok(news);
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
                var news = await _newsService.GetNewsById(id);
                return Ok(news);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, NewsUpdateRequestDto requestDto)
        {
            try
            {
                var news = await _newsService.GetNewsById(id);
                news = await _newsService.UpdateNews(id, requestDto);
                return Ok(news);
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
                var news = await _newsService.GetNewsById(id);
                news = await _newsService.DeleteNews(id);
                return Ok(news);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(NewsRequestDto requestDto)
        {
            try
            {
                var news = await _newsService.CreateNews(requestDto);
                return Ok(news);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCSV()
        {
            try
            {
                var csvBytes = await _newsService.ExportContentToCSVAsync();
                var fileName = $"NewsListing_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var fileContents = await _newsService.ExportContentToExcelAsync();
                var fileName = $"NewsListing_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

                return File(fileContents, 
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                            fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}