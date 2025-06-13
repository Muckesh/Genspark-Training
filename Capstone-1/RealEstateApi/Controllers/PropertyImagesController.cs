using System.Security.Claims;
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
    public class PropertyImagesController : ControllerBase
    {
        private readonly IPropertyImageService _propertyImageService;
        private readonly IPropertyListingService _propertyListingService;

        private readonly IImageCleanupService _imageCleanupService;
        public PropertyImagesController(IPropertyImageService propertyImageService, IPropertyListingService propertyListingService, IImageCleanupService imageCleanupService)
        {
            _propertyImageService = propertyImageService;
            _propertyListingService = propertyListingService;
            _imageCleanupService = imageCleanupService;
        }

       

        [HttpGet("file/{imageId}")]
        public async Task<IActionResult> GetImageFile(Guid imageId)
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _propertyImageService.GetImageFileAsync(imageId);
                return File(fileBytes, contentType, fileName);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("urls/{listingId}")]
        public async Task<IActionResult> GetListingImageUrls(Guid listingId)
        {
            try
            {
                var urls = await _propertyImageService.GetImageUrlsByListingIdAsync(listingId);
                return Ok(urls);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpDelete("admin/images/cleanup")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CleanupOldDeletedImages()
        {
            try
            {
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                await _imageCleanupService.CleanOldDeletedImagesAsync(basePath);
                return Ok("Old deleted images cleaned up");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{imageId}")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> SoftDeletePropertyImage(Guid imageId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Optional: Ensure agent owns the listing of the image
                var image = await _propertyImageService.GetImageByIdAsync(imageId);
                // if (image == null) return NotFound("Image not found");

                var listing = await _propertyListingService.GetListingByIdAsync(image.PropertyListingId);
                if (listing == null || listing.AgentId.ToString() != userId)
                    return Forbid("You do not have permission to delete this image");

                var success = await _propertyImageService.SoftDeleteImageAsync(imageId);
                if (!success) return StatusCode(500, "Unable to delete image");

                return Ok("Image marked as deleted");
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        

    }
}