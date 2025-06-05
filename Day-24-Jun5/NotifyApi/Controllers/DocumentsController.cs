using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : Controller
{
    private readonly IDocumentService _documentService;


    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;

    }

    [HttpPost("upload")]
    [Authorize(Roles ="HR")]
    public async Task<ActionResult<string>> UploadDocument([FromForm] DocumentUploadDto file)
    {
        try
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _documentService.PostFile(file, user);
            return result;
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }


    [HttpGet("GetFile")]
    [Authorize]
    public async Task<ActionResult<DocumentGetDto>> GetFile(int id)
    {
        try
        {
            var result = await _documentService.DownloadFileById(id);
            return result;
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
}