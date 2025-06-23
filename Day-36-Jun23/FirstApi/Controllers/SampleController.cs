using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly IFileProcessingService _processingService;
    public SampleController(IFileProcessingService processingService)
    {
        _processingService = processingService;
    }
    
    [HttpPost("FromCsv")]
    public async Task<IActionResult> BulkInsertFromCsv([FromBody] CsvUploadDto input)
    {
        return Ok(await _processingService.ProcessData(input));
    }
}