using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IPropertyImageService
    {
        Task<PropertyImage> UploadPropertyImageAsync(AddPropertyImageDto imageDto);
        Task<IEnumerable<PropertyImage>> GetImagesByListingIdAsync(Guid listingId);
        Task<bool> SoftDeleteImageAsync(Guid imageId);
        // Task CleanOldDeletedImagesAsync(string basePath, int retentionDays = 30);
        Task<PropertyImage> GetImageByIdAsync(Guid imageId);
        Task<int> DeleteImagesByListingIdAsync(Guid listingId);
        Task<(byte[] FileBytes, string ContentType, string FileName)> GetImageFileAsync(Guid imageId);

        Task<IEnumerable<string>> GetImageUrlsByListingIdAsync(Guid listingId);
        Task<IEnumerable<(byte[] FileBytes, string ContentType, string FileName)>> GetImageFilesAsync(Guid listingId);


    }
}