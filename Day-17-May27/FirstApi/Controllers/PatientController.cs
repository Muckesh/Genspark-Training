using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class PatientController : ControllerBase
{
    static List<Patient> patients = new List<Patient>
    {
        new Patient{Id = 101,Name="Alice",Age = 23 },
        new Patient{Id = 102, Name = "Bob", Age = 30}
    };

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetPatients()
    {
        return Ok(patients);
    }

    [HttpPost]
    public ActionResult<Doctor> CreatePatient([FromBody] Patient patient)
    {
        patients.Add(patient);
        return Created("", patient);
    }

    [HttpPut("{id}")]
    public ActionResult<Doctor> UpdatePatient(int id, [FromBody] Patient patient)
    {
        var existingPatient = patients.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
        {
            return NotFound();
        }
        patient.Id = existingPatient.Id;
        existingPatient.Name = patient.Name;
        existingPatient.Age = patient.Age;
        // existingPatient.Diagnosis = patient.Diagnosis;
        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public ActionResult<Doctor> DeletePatient(int id)
    {
        var existingPatient = patients.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
        {
            return NotFound();
        }
        patients.Remove(existingPatient);
        return Ok(existingPatient);
    }
}