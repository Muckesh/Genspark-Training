using Microsoft.AspNetCore.Mvc;
using VideoPortalAPi.Interfaces;
using VideoPortalAPi.Models.DTOs;

namespace VideoPortalAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto fileUploadDto)
        {
            var video = await _videoService.UploadVideoAsync(fileUploadDto.Video, fileUploadDto.Title, fileUploadDto.Description);
            return Ok(video);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return Ok(videos);
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> GetVideoById(Guid id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if(video==null)
                return NotFound();

            return Ok(video.BlobUrl);
        }
    }
}