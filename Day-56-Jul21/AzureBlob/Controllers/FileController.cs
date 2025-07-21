using AzureBlob.DTOs;
using AzureBlob.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;

        public FileController(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<Stream>> Download(string fileName)
        {
            var stream = await _blobStorageService.DownloadFile(fileName);
            if (stream == null)
                return NotFound();
            return File(stream, "application/octet-stream", fileName);
        }

        [Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadRequestDto requestDto)
        {
            if (requestDto.File == null || requestDto.File.Length == 0)
                return BadRequest("No file to upload");
            using var stream = requestDto.File.OpenReadStream();
            await _blobStorageService.UploadFile(stream, requestDto.File.FileName);
            return Ok("File Uploaded");
        }
    }
}