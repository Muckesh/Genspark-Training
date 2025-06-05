using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] FileUploadDto dto)
    {
        var uploadedFile = await _fileService.SaveFile(dto.File);
        return Ok(new { uploadedFile.Id, uploadedFile.FileName });
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var file = _fileService.GetFile(id);
        if (file == null) return NotFound();

        return File(file.Data, file.ContentType, file.FileName);
    }
}