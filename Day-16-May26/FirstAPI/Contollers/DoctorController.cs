using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("/api/[controller]")]
public class DoctorController : ControllerBase
{
    // List<Doctor> doctors = new List<Doctor> -> without static each time get or post method is called, object will be created and disposed.
    // so the list will not be updated.
    static List<Doctor> doctors = new List<Doctor>
    {
        new Doctor{Id = 101, Name="Ramu"},
        new Doctor{Id=102, Name="Somu"}
    };

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetDoctors()
    {
        return Ok(doctors);
    }

    [HttpPost]
    public ActionResult<Doctor> CreateDoctor([FromBody] Doctor doctor)
    {
        doctors.Add(doctor);
        return Created("", doctor);
    }

    [HttpPut("{id}")]
    public ActionResult<Doctor> UpdateDoctor(int id, [FromBody] Doctor doctor)
    {
        var existingDoctor = doctors.FirstOrDefault(d => d.Id == id);
        if (existingDoctor == null)
        {
            return NotFound();
        }
        existingDoctor.Name = doctor.Name;

        return Ok(doctor);
    }

    [HttpDelete("{id}")]
    public ActionResult<Doctor> DeleteDoctor(int id)
    {
        var existingDoctor = doctors.FirstOrDefault(d => d.Id == id);
        if (existingDoctor == null)
        {
            return NotFound();
        }
        doctors.Remove(existingDoctor);
        return Ok(existingDoctor);
    }
    
}