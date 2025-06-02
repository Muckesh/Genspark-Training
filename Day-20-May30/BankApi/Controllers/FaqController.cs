using BankApi.Models;
using BankApi.Models.DTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly FaqService _faqService;

        public FaqController(FaqService faqService)
        {
            _faqService = faqService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] FAQRequestDto request)
        {
            try
            {
                var result = await _faqService.AskQuestion(request.Question);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}