using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;
        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var contacts = await _contactUsService.GetAllContacts();
                return Ok(contacts);
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
                var contact = await _contactUsService.GetContactById(id);
                return Ok(contact);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContactRequestDto requestDto)
        {
            try
            {
                var contact = await _contactUsService.GetContactById(id);
                contact = await _contactUsService.UpdateContact(id, requestDto);
                return Ok(contact);
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
                var contact = await _contactUsService.GetContactById(id);
                contact = await _contactUsService.DeleteContact(id);
                return Ok(contact);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ContactRequestDto requestDto)
        {
            try
            {
                var captchaResult = await _contactUsService.VerifyTokenAsync(requestDto.RecaptchaToken);
                if (!captchaResult.Success)
                {
                    return BadRequest(new
                    {
                        Message = "reCAPTCHA validation failed",
                        Errors = captchaResult.ErrorCodes
                    });
                }
        
                var contact = await _contactUsService.CreateContact(requestDto);
                return Ok(contact);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}