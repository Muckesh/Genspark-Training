using Azure.Storage.Blobs;

namespace AzureBlob.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var SasUrl = configuration["AzureBlob:SasUrl"];
            _containerClient = new BlobContainerClient(new Uri(SasUrl));
        }

        public async Task UploadFile(Stream fileStream, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadFile(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync())
            {
                var downloadInFor = await blobClient.DownloadStreamingAsync();
                return downloadInFor.Value.Content;
            }
            return null;
        }
    }
}