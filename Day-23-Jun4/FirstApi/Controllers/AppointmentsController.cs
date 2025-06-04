using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> Add(AppointmentAddRequestDto appointment)
    {
        try
        {
            var newAppointment = await _appointmentService.AddAppointment(appointment);
            if (newAppointment != null)
                return Created("", newAppointment);
            return BadRequest("Unable to process the request.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<Appointment>>> AllAppointments()
    {
        try
        {
            var appointments = await _appointmentService.GetAllAppointments();
            if (appointments == null)
            {
                return BadRequest("No appointments in the database");
            }
            return Created("", appointments);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("cancel")]
    [Authorize(Policy ="ExperiencedDocOnly")]
    public async Task<ActionResult<Appointment>> Cancel(int appointmentNumber)
    {
        try
        {
            var appointment = await _appointmentService.CancelAppointment(appointmentNumber);
            if (appointment == null)
                return BadRequest("Unable to cancel the appointment at the moment");
            return appointment;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}