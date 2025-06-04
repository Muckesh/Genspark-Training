
using Microsoft.EntityFrameworkCore;

public class AppointmentRepository : Repository<int, Appointment>
{
    public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
    {

    }

    public override async Task<Appointment> Get(int key)
    {
        var appointment = await _clinicContext.Appointments.SingleOrDefaultAsync(a=>a.AppointmentNumber==key);
        return appointment ?? throw new Exception("Appointment not found with the given ID.");
    }

    public override async Task<IEnumerable<Appointment>> GetAll()
    {
        var appointments = _clinicContext.Appointments;
        if (appointments.Count() == 0)
        {
            throw new Exception("No appointments in the database.");
        }
        return (await appointments.ToListAsync());
    }
}