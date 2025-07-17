using System.Runtime.Intrinsics.Arm;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using AzureBlob.DTOs;

namespace AzureBlob.Services
{
    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<BlobStorageService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private async Task<BlobClient> GetBlobClientWithSas(string fileName)
        {
            string functionUrl = $"https://muckeshfunc.azurewebsites.net/api/generate-sas/{fileName}?code=<fn-key>";
            var client = _httpClientFactory.CreateClient();
            var sasResponse = await client.GetAsync(functionUrl);
            if (!sasResponse.IsSuccessStatusCode)
            {
                var error = await sasResponse.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get SAS URL: {error}");
                throw new InvalidOperationException("Could not obtain SAS URL.");
            }

            var sasData = await sasResponse.Content.ReadFromJsonAsync<SasResponse>();
            if (sasData == null || string.IsNullOrWhiteSpace(sasData.sasUrl))
            {
                throw new InvalidOperationException("SAS URL response invalid.");
            }

            _logger.LogInformation($"SAS URL obtained: {sasData.sasUrl}");

            // Create BlobClient directly using the SAS URL
            return new BlobClient(new Uri(sasData.sasUrl));
        }


        // private async Task UpdateContainerClient()
        // {
        //     var blobUrl = _configuration["AzureBlob:VaultUrl"];
        //     SecretClient secretClient = new SecretClient(new Uri(blobUrl), new DefaultAzureCredential());
        //     // KeyVaultSecret secret = await secretClient.GetSecretAsync("SasUrl");
        //     KeyVaultSecret secret = await secretClient.GetSecretAsync("ConnectionString");
        //     // var blobUrlValue = secret.Value;
        //     // _containerClient = new BlobContainerClient(new Uri(blobUrlValue));
        //     var connectionString = secret.Value;
        //     _containerClient = new BlobContainerClient(connectionString, "images");
        // }

        public async Task UploadFile(Stream fileStream, string fileName)
        {
            // await UpdateContainerClient();
            var blobClient = await GetBlobClientWithSas(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadFile(string fileName)
        {
            // await UpdateContainerClient();
            var blobClient = await GetBlobClientWithSas(fileName);
            if (await blobClient.ExistsAsync())
            {
                var downloadInFor = await blobClient.DownloadStreamingAsync();
                return downloadInFor.Value.Content;
            }
            return null;
        }
    }
}