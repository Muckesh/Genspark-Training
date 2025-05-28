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

    [HttpGet]
    public async Task<IActionResult> GetDoctors()
    {
        try
        {
            var doctors = await _doctorService.GetAllDoctors();
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

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

    [HttpGet("speciality/{specialityName}")]
    public async Task<IActionResult> GetDoctorsBySpeciality(string specialityName)
    {
        try
        {
            var doctors = await _doctorService.GetDoctorsBySpeciality(specialityName);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddDoctor([FromBody] DoctorAddRequestDto doctorDto)
    {
        try
        {
            var addedDoctor = await _doctorService.AddDoctor(doctorDto);
            return CreatedAtAction(nameof(GetDoctorByName), new { doctorName = addedDoctor.Name }, addedDoctor);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
