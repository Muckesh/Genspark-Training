using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("patients")]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<IEnumerable<Patient>>> GetDoctors()
    {
        try
        {
            var patients = await _patientService.GetAllPatients();
            return patients.ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}