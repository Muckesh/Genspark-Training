using VideoPortalAPi.Models;

namespace VideoPortalAPi.Interfaces
{
    public interface IVideoService
    {
        Task<TrainingVideo> UploadVideoAsync(IFormFile file, string title, string description);
        Task<List<TrainingVideo>> GetAllVideosAsync();
        Task<TrainingVideo?> GetVideoByIdAsync(Guid id);
    }
}