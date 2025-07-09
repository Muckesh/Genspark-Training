using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class InquiriesController : ControllerBase
    {
        private readonly IInquiryService _inquiryService;

        public InquiriesController(IInquiryService inquiryService)
        {
            _inquiryService = inquiryService;
        }

        [HttpGet]
        // [Authorize(Roles = "Agent")]
        public async Task<IActionResult> GetFilteredInquiries([FromQuery] InquiryQueryDto query)
        {
            try
            {
                var result = await _inquiryService.GetFilteredInquiriesAsync(query);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> AddInquiry(AddInquiryDto inquiryDto)
        {
            try
            {
                var inquiry = await _inquiryService.CreateInquiryAsync(inquiryDto);
                return Created("", inquiry);
            }
            catch (UnauthorizedAccessAppException e)
            {
                return Forbid(e.Message);
            }
            catch (ArgumentRequiredException e)
            {
                return BadRequest(e.Message);
            }
            catch (FailedOperationException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("existing")]
        public async Task<IActionResult> GetExistingInquiry([FromQuery] Guid listingId, [FromQuery] Guid buyerId)
        {
            var inquiry = await _inquiryService.GetExistingInquiryAsync(listingId, buyerId);
            if (inquiry == null)
                return NotFound();
            return Ok(inquiry);
        }



        [HttpGet("{inquiryId}/replies")]
        [Authorize(Roles = "Agent,Buyer")]
        public async Task<IActionResult> GetInquiryReplies(Guid inquiryId)
        {
            try
            {
                var replies = await _inquiryService.GetRepliesAsync(inquiryId);
                return Ok(replies);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{inquiryId}/with-replies")]
        [Authorize(Roles = "Agent,Buyer")]
        public async Task<IActionResult> GetInquiryWithReplies(Guid inquiryId)
        {
            try
            {
                var result = await _inquiryService.GetInquiryWithRepliesAsync(inquiryId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{inquiryId}/replies")]
        [Authorize(Roles = "Agent,Buyer")]
        public async Task<IActionResult> AddReply(Guid inquiryId, [FromBody] AddReplyDto replyDto)
        {
            try
            {
                var reply = await _inquiryService.AddReplyAsync(inquiryId, replyDto);
                return CreatedAtAction(nameof(GetInquiryReplies), new { inquiryId }, reply);
            }
            catch (UnauthorizedAccessAppException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        
    }
}
