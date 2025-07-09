using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;

namespace RealEstateApi.Services
{
    public class ImageCleanUpService : IImageCleanupService
    {
        private readonly RealEstateDbContext _realEstateDbContext;
        private readonly IRepository<Guid, PropertyImage> _propertyImageRepository;

        public ImageCleanUpService(RealEstateDbContext realEstateDbContext, IRepository<Guid, PropertyImage> propertyImageRepository)
        {
            _realEstateDbContext = realEstateDbContext;
            _propertyImageRepository = propertyImageRepository;
        }
        public async Task<int> CleanOldDeletedImagesAsync(string basePath, int retentionDays = 1)
        {
            var allImages = await _realEstateDbContext.PropertyImages.ToListAsync();
            var cutoffDate = DateTime.UtcNow.AddMinutes(-1);

            Console.WriteLine($"[Cleanup] Cutoff date is {cutoffDate}");

            var imagesToDelete = allImages
                .Where(i => !i.IsHardDeleted && i.IsDeleted && i.DeletedAt.HasValue && i.DeletedAt.Value <= cutoffDate);
            Console.WriteLine($"[Cleanup] Found {imagesToDelete.Count()} images to delete.");

            int successCount = 0;
            int failCount = 0;

            foreach (var image in imagesToDelete)
            {
                try
                {
                    var listingFolder = Path.Combine(basePath, "property-images", image.PropertyListingId.ToString());
                    var filePath = Path.Combine(listingFolder, image.FileName);
                    Console.WriteLine($"Deleting file: {filePath}");

                    if (File.Exists(filePath))
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);

                        File.Delete(filePath);
                        Console.WriteLine("Deleted successfully.");
                        successCount++;
                    }
                    else
                    {
                        Console.WriteLine("File not found: " + filePath);
                        failCount++;
                    }

                    image.IsHardDeleted = true;

                    var deleted = await _propertyImageRepository.UpdateAsync(image.Id, image);

                    // deleted = await _propertyImageRepository.DeleteAsync(image.Id); // Hard delete from DB

                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error deleting image {image.Id}: {ex.Message}");
                    failCount++;
                }

            }
            return successCount;
        }
    }
}