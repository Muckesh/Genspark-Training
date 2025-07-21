using Microsoft.EntityFrameworkCore;
using VideoPortalAPi.Models;

namespace VideoPortalAPi.Contexts
{
    public class VideoPortalDbContext : DbContext
    {
        public DbSet<TrainingVideo> TrainingVideos { get; set; }
        
        public VideoPortalDbContext(DbContextOptions<VideoPortalDbContext> options) : base(options)
        {
            
        }
    }
}