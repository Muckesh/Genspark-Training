using Azure.Storage.Blobs;
using VideoPortalAPi.Interfaces;
using VideoPortalAPi.Models;

namespace VideoPortalAPi.Services
{
    public class VideoService : IVideoService
    {
        private readonly IRepository<Guid, TrainingVideo> _trainingVideoRepository;

        private readonly IConfiguration _configuration;

        public VideoService(IRepository<Guid, TrainingVideo> trainingVideoRepository, IConfiguration configuration)
        {
            _trainingVideoRepository = trainingVideoRepository;
            _configuration = configuration;
        }
        public async Task<List<TrainingVideo>> GetAllVideosAsync()
        {
            var videos = await _trainingVideoRepository.GetAll();
            return videos.ToList();
        }

        public async Task<TrainingVideo?> GetVideoByIdAsync(Guid id)
        {
            var video = await _trainingVideoRepository.GetById(id);
            return video;
        }

        public async Task<TrainingVideo> UploadVideoAsync(IFormFile file, string title, string description)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be empty.");

            var containerName = _configuration["AzureBlobStorage:ContainerName"];
            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var blobContainerClient = new BlobContainerClient(connectionString, containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
            await blobContainerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            var video = new TrainingVideo
            {
                Title = title,
                Description = description,
                BlobUrl = blobClient.Uri.ToString(),
                UploadDate = DateTime.UtcNow
            };

            video = await _trainingVideoRepository.Add(video);
            return video;
        }
    }
}