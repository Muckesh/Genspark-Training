using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentsController(IDocumentService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "HR")]
    public async Task<IActionResult> Upload([FromForm] DocumentUploadRequestDto request)
    {
        try
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.UploadDocumentAsync(request, user);
            return Ok(result);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDocument(int id)
    {
        var doc = await _service.GetDocumentsAsync(id);
        return Ok(doc);
    }
}
