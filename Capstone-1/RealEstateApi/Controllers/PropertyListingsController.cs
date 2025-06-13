using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PropertyListingsController : ControllerBase
    {
        private readonly IPropertyListingService _propertyListingService;
        private readonly IPropertyImageService _propertyImageService;
        // private readonly IImageCleanupService _imageCleanupService;

        public PropertyListingsController(IPropertyListingService propertyListingService, IPropertyImageService propertyImageService)
        {
            _propertyListingService = propertyListingService;
            _propertyImageService = propertyImageService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchListings([FromQuery] PropertyListingQueryParametersDto query)
        {
            try
            {
                var listings = await _propertyListingService.GetFilteredListingsAsync(query);
                return Ok(listings);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(Guid id)
        {
            try
            {
                var listing = await _propertyListingService.GetListingByIdAsync(id);
                return Ok(listing);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateListing(CreatePropertyListingDto listingDto)
        {
            try
            {
                var listing = await _propertyListingService.AddListingAsync(listingDto);
                return Created("", listing);
            }
            catch (UnauthorizedAccessAppException e)
            {
                return Unauthorized(e.Message);
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
        
        [HttpPost("images/upload")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> UploadImage([FromForm] AddPropertyImageDto dto)
        {
            try
            {
                // var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var image = await _propertyImageService.UploadPropertyImageAsync(dto);
                return Ok(image);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (FailedOperationException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (UnauthorizedAccessAppException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Agent")]
        public async Task<ActionResult<PropertyListing>> UpdateListing(Guid id, [FromBody] UpdatePropertyListingDto listingDto)
        {
            try
            {
                // Get current agent ID from JWT
                var agentId = User.GetUserId();
                if (!agentId.HasValue)
                {
                    return Unauthorized("Invalid user token.");
                }

                // Fetch the listing
                var listing = await _propertyListingService.GetListingByIdAsync(id);
                if (listing == null)
                {
                    return NotFound("Listing not found.");
                }

                // Check if the current agent owns the listing
                if (listing.AgentId != agentId)
                {
                    return Forbid("You are not authorized to update this listing.");
                }

                // Proceed to update
                var updatedListing = await _propertyListingService.UpdateListingAsync(id, listingDto);
                return Ok(updatedListing);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message );
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Agent")]
        public async Task<ActionResult<PropertyListing>> DeleteListing(Guid id)
        {
            try
            {
                // Get agent ID from JWT token
                var agentId = User.GetUserId();
                if (!agentId.HasValue)
                {
                    return Unauthorized("Invalid user token.");
                }

                // Get the listing
                var listing = await _propertyListingService.GetListingByIdAsync(id);
                if (listing == null || listing.IsDeleted)
                {
                    return NotFound("Listing not found.");
                }

                // Ensure only the owning agent can delete it
                if (listing.AgentId != agentId)
                {
                    return Forbid("You are not authorized to delete this listing.");
                }

                var deletedListing = await _propertyListingService.DeleteListingAsync(id);
                return Ok(deletedListing);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        

        

    }
}

