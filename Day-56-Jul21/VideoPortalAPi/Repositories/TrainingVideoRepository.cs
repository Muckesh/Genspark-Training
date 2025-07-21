using Microsoft.EntityFrameworkCore;
using VideoPortalAPi.Contexts;
using VideoPortalAPi.Models;

namespace VideoPortalAPi.Repositories
{
    public class TrainingVideoRepository : Repository<Guid, TrainingVideo>
    {
        public TrainingVideoRepository(VideoPortalDbContext videoPortalDbContext): base(videoPortalDbContext)
        {
            
        }
        public override async Task<ICollection<TrainingVideo>> GetAll()
        {
            var videos = await _videoPortalDbContext.TrainingVideos.ToListAsync();
            return videos;
        }

        public override async Task<TrainingVideo> GetById(Guid id)
        {
            var video = await _videoPortalDbContext.TrainingVideos.SingleOrDefaultAsync(t => t.Id == id);
            return video ?? throw new KeyNotFoundException("Video not found.");
        }
    }
}