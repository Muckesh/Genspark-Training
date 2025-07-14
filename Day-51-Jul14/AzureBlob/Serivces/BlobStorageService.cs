using System.Runtime.Intrinsics.Arm;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;

namespace AzureBlob.Services
{
    public class BlobStorageService
    {
        private BlobContainerClient _containerClient;
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            // var SasUrl = configuration["AzureBlob:SasUrl"];
            // _containerClient = new BlobContainerClient(new Uri(SasUrl));
        }

        private async Task UpdateContainerClient()
        {
            var blobUrl = _configuration["AzureBlob:VaultUrl"];
            SecretClient secretClient = new SecretClient(new Uri(blobUrl), new DefaultAzureCredential());
            // KeyVaultSecret secret = await secretClient.GetSecretAsync("SasUrl");
            KeyVaultSecret secret = await secretClient.GetSecretAsync("ConnectionString");
            // var blobUrlValue = secret.Value;
            // _containerClient = new BlobContainerClient(new Uri(blobUrlValue));
            var connectionString = secret.Value;
            _containerClient = new BlobContainerClient(connectionString, "images");
        }

        public async Task UploadFile(Stream fileStream, string fileName)
        {
            await UpdateContainerClient();
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadFile(string fileName)
        {
            await UpdateContainerClient();
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