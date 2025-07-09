using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using System.Security.Claims;

namespace RealEstateApi.Services
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IRepository<Guid, PropertyImage> _propertyImageRepository;
        private readonly IRepository<Guid, PropertyListing> _propertyListingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyImageService(
            IRepository<Guid, PropertyImage> propertyImageRepository,
            IRepository<Guid, PropertyListing> propertyListingRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _propertyImageRepository = propertyImageRepository;
            _propertyListingRepository = propertyListingRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PropertyImage> UploadPropertyImageAsync(AddPropertyImageDto imageDto)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var listing = await _propertyListingRepository.GetByIdAsync(imageDto.PropertyListingId);
            if (listing == null || listing.IsDeleted)
                throw new NotFoundException("Property listing not found.");

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!userId.HasValue && (userRole!="Agent" || userRole!="Admin"))
                throw new UnauthorizedAccessAppException("Unauthorized access.");

            // Guid agentId;

            // if (userRole == "Admin")
            // {
                // if (listing.AgentId==null)
                //     throw new ArgumentRequiredException("AgentId is required when Admin is creating a listing.");

            //     agentId = listing.AgentId;
            // }

            // Agent check
            // var agentId = _httpContextAccessor.HttpContext?.User.GetUserId();
            // if (!agentId.HasValue || agentId != listing.AgentId)
            //     throw new UnauthorizedAccessAppException("You are not authorized to upload images for this listing.");

            // Sanitize and generate unique file name
            var originalFileName = Path.GetFileName(imageDto.ImageFile.FileName);
            var extension = Path.GetExtension(originalFileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (!allowedExtensions.Contains(extension.ToLower()))
                throw new FailedOperationException("Unsupported image format.");

            // Save image
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var listingFolder = Path.Combine(basePath, "property-images", listing.Id.ToString());

            if (!Directory.Exists(listingFolder))
                Directory.CreateDirectory(listingFolder);

            var filePath = Path.Combine(listingFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageDto.ImageFile.CopyToAsync(stream);
            }

            var image = new PropertyImage
            {
                FileName = uniqueName,
                FileUrl = $"/property-images/{listing.Id}/{uniqueName}",
                PropertyListingId = listing.Id
            };

            image = await _propertyImageRepository.AddAsync(image);

            return image ?? throw new FailedOperationException("Unable to save property image.");
        }

        public async Task<IEnumerable<PropertyImage>> GetImagesByListingIdAsync(Guid listingId)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(listingId);
            if (listing == null || listing.IsDeleted)
                throw new NotFoundException("Listing not found.");

            var allImages = await _propertyImageRepository.GetAllAsync();
            return allImages.Where(i => i.PropertyListingId == listingId);
        }

        public async Task<bool> SoftDeleteImageAsync(Guid imageId)
        {
            var image = await _propertyImageRepository.GetByIdAsync(imageId);
            if (image == null) throw new NotFoundException("Image not found");

            image.IsDeleted = true;
            image.DeletedAt = DateTime.UtcNow;

            var updated = await _propertyImageRepository.UpdateAsync(image.Id, image);
            return updated != null;
        }

        // public async Task CleanOldDeletedImagesAsync(string basePath, int retentionDays = 1)
        // {
        //     var allImages = await _propertyImageRepository.GetAllAsync();
        //     var cutoffDate = DateTime.UtcNow.AddHours(-1);

        //     var imagesToDelete = allImages
        //         .Where(i => i.IsDeleted && i.DeletedAt.HasValue && i.DeletedAt.Value <= cutoffDate);

        //     foreach (var image in imagesToDelete)
        //     {
        //         try
        //         {
        //             var listingFolder = Path.Combine(basePath, "property-images", image.PropertyListingId.ToString());
        //             var filePath = Path.Combine(listingFolder, image.FileName);
        //             if (File.Exists(filePath))
        //             {
        //                 File.SetAttributes(filePath, FileAttributes.Normal);

        //                 File.Delete(filePath);
        //             }
        //             await _propertyImageRepository.DeleteAsync(image.Id); // Hard delete from DB
        //         }
        //         catch (Exception ex)
        //         {
        //             // Log error, continue cleanup
        //             Console.WriteLine($"Error deleting image {image.Id}: {ex.Message}");
        //         }
        //     }
        // }

        public async Task<PropertyImage> GetImageByIdAsync(Guid imageId)
        {
            var image = await _propertyImageRepository.GetByIdAsync(imageId);

            if (image == null || image.IsDeleted)
                throw new NotFoundException("Image not found or has been deleted.");

            return image;
        }

        public async Task<int> DeleteImagesByListingIdAsync(Guid listingId)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(listingId);
            if (listing == null || listing.IsDeleted)
                throw new NotFoundException("Listing not found or has been deleted.");

            var allImages = await _propertyImageRepository.GetAllAsync();
            var imagesToDelete = allImages.Where(i => i.PropertyListingId == listingId && !i.IsDeleted).ToList();

            int deleteCount = 0;
            foreach (var image in imagesToDelete)
            {
                image.IsDeleted = true;
                image.DeletedAt = DateTime.UtcNow;
                var updated = await _propertyImageRepository.UpdateAsync(image.Id, image);
                if (updated != null) deleteCount++;
            }

            return deleteCount;
        }

        public async Task<(byte[] FileBytes, string ContentType, string FileName)> GetImageFileAsync(Guid imageId)
        {
            var image = await _propertyImageRepository.GetByIdAsync(imageId);

            if (image == null || image.IsDeleted)
                throw new NotFoundException("Image not found or has been deleted.");

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(basePath, "property-images", image.PropertyListingId.ToString(), image.FileName);

            if (!File.Exists(fullPath))
                throw new NotFoundException("Image file not found on disk.");

            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            var contentType = GetContentType(image.FileName);

            return (fileBytes, contentType, image.FileName);
        }
        public async Task<IEnumerable<(byte[] FileBytes, string ContentType, string FileName)>> GetImageFilesAsync(Guid listingId)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(listingId);
            if (listing == null || listing.IsDeleted)
                throw new NotFoundException("Listing not found or has been deleted.");

            var allImages = await _propertyImageRepository.GetAllAsync();
            var images = allImages
                .Where(i => i.PropertyListingId == listingId && !i.IsDeleted)
                .ToList();

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var results = new List<(byte[] FileBytes, string ContentType, string FileName)>();

            foreach (var image in images)
            {
                var filePath = Path.Combine(basePath, "property-images", listingId.ToString(), image.FileName);

                if (!File.Exists(filePath))
                    continue; // Optionally log missing files

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(image.FileName);

                results.Add((fileBytes, contentType, image.FileName));
            }

            return results;
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        

        public async Task<IEnumerable<string>> GetImageUrlsByListingIdAsync(Guid listingId)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(listingId);
            if (listing == null || listing.IsDeleted)
                throw new NotFoundException("Listing not found or has been deleted.");

            var allImages = await _propertyImageRepository.GetAllAsync();
            var images = allImages.Where(i => i.PropertyListingId == listingId && !i.IsDeleted).ToList();

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new NotFoundException("HTTP context not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var imageUrls = images.Select(i =>
                $"{baseUrl}/property-images/{listingId}/{i.FileName}");

            return imageUrls;
        }




    }
}
