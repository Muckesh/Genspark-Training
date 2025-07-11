namespace RealEstateApi.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file, string blobName);
        Task<bool> DeleteFileAsunc(string blobName);
        Task<Stream> GetFileStreamAsync(string blobName);
        string GetBlobUrl(string blobName);
    }
}