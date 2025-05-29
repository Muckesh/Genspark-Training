using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    // [HttpGet]
    // public async Task<IActionResult> GetDoctors()
    // {
    //     try
    //     {
    //         var doctors = await _doctorService.GetAllDoctors();
    //         return Ok(doctors);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex.Message);
    //     }
    // }

    [HttpGet("name/{doctorName}")]
    public async Task<IActionResult> GetDoctorByName(string doctorName)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByName(doctorName);
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorsBySpecialityResponseDto>>> GetDoctorsBySpecialityName(string speciality)
    {
        try
        {
            var result = await _doctorService.GetDoctorsBySpeciality(speciality);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Doctor>> AddDoctor([FromBody] DoctorAddRequestDto doctorDto)
    {
        try
        {
            var newDoctor = await _doctorService.AddDoctor(doctorDto);
            if (newDoctor != null)
            {
                return Created("", newDoctor);
            }
            return BadRequest("Unable to process the request at this moment.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
