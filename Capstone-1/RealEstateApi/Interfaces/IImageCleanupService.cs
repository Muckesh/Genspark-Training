namespace RealEstateApi.Interfaces
{
    public interface IImageCleanupService
    {
        Task CleanOldDeletedImagesAsync(string basePath, int retentionDays = 30);

    }
}