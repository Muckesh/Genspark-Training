using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }
    [HttpPost]
    public async Task<ActionResult<Patient>> AddPatient(PatientAddRequestDto requestDto)
    {
        try
        {
            var patient = await _patientService.AddPatient(requestDto);
            if (patient == null)
                return BadRequest("Unable to process the request.");
            return Created("", patient);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}