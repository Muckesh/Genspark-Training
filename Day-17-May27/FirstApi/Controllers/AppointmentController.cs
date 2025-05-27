using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Appointment>> GetAppointments()
    {
        var result = _appointmentService.GetAppointments();
        if (result == null)
        {
            return NotFound("No appointments found.");
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<Appointment> GetAppointment(int id)
    {
        var appointment = _appointmentService.GetAppointment(id);
        if (appointment == null)
        {
            return NotFound();
        }
        return Ok(appointment);
    }

    [HttpPost]
    public ActionResult<Appointment> CreateAppointment([FromBody] Appointment appointment)
    {
        _appointmentService.AddAppointment(appointment);
        return Created("", appointment);
    }

    // [HttpPut("{id}")]
    // public ActionResult<Appointment> UpdateAppointment(int id, [FromBody] Appointment appointment)
    // {
    //     var existingAppointment = _appointmentService.GetAppointment(id);
    //     if (existingAppointment == null)
    //     {
    //         return NotFound();
    //     }
    //     appointment.Id = id;
    //     _appointmentService.UpdateAppointment(appointment);
    //     return Ok(appointment);
    // }

    [HttpDelete("{id}")]
    public ActionResult<Appointment> DeleteAppointment(int id)
    {
        var existingAppointment = _appointmentService.GetAppointment(id);
        if (existingAppointment == null)
        {
            return NotFound();
        }
        _appointmentService.DeleteAppointment(id);
        return Ok(existingAppointment);
    }
}