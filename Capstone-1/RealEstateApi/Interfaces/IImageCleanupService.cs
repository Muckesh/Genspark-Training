namespace RealEstateApi.Interfaces
{
    public interface IImageCleanupService
    {
        Task<int> CleanOldDeletedImagesAsync(string basePath, int retentionDays = 30);

    }
}