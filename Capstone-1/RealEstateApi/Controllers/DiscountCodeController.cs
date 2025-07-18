using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DiscountCodeController : ControllerBase
    {
        private readonly IDiscountCodeService _discountCodeService;

        public DiscountCodeController(IDiscountCodeService discountCodeService)
        {
            _discountCodeService = discountCodeService;
        }

        [HttpGet("Get")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DiscountCodes>>> GetAllCodes()
        {
            try
            {
                var result = await _discountCodeService.GetCodes();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Check")]
        [Authorize]
        public async Task<ActionResult<DiscountCodes>> CheckCodeExist(string code)
        {

            try
            {
                var result = await _discountCodeService.CheckCodeAvail(code);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountCodes>> AddCodes(AddDiscountCodeDto codes)
        {
            try
            {
                var result = await _discountCodeService.AddDiscountCode(codes);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCode(Guid id)
        {
            var result = await _discountCodeService.SoftDeleteCode(id);
            if (result)
            {
                return NoContent();

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPatch("Expire")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountCodes>> PatchCodeExpire(Guid id)
        {
            var result = await _discountCodeService.PatchCodeExpire(id);
            if (result!=null)
            {
                return Ok(result);

            }
            else
            {
                return BadRequest();
            }
        }
    }
}