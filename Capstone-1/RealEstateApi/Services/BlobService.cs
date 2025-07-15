using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using RealEstateApi.Interfaces;

namespace RealEstateApi.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobService(IConfiguration configuration)
        {
            var vaultUrl = configuration["AzureBlobStorage:VaultUrl"];

            string? connectionString = null;

            var client = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret("ConnectionString");

            connectionString = secret.Value;

            // var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            // var containerName = configuration["AzureBlobStorage:ContainerName"];
            var containerName = "property-images";
            _blobContainerClient = new BlobContainerClient(connectionString, containerName);
            _blobContainerClient.CreateIfNotExists(PublicAccessType.Blob);
        }
        public async Task<bool> DeleteFileAsunc(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            return await blobClient.DeleteIfExistsAsync();
        }

        public string GetBlobUrl(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }

        public async Task<Stream> GetFileStreamAsync(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            var response = await blobClient.DownloadStreamingAsync();
            return response.Value.Content;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);
            return blobClient.Uri.ToString();
        }


    }
}