
using Microsoft.EntityFrameworkCore;

public class DoctorRepository : Repository<int, Doctor>
{
    public DoctorRepository(ClinicContext clinicContext) : base(clinicContext)
    {
        
    }
    public override async Task<Doctor> Get(int key)
    {
        var doctor = await _clinicContext.Doctors.SingleOrDefaultAsync(d => d.Id == key);
        if (doctor != null)
        {
            return doctor;
        }
        throw new Exception("Doctor not found with the given ID.");
    }

    public override async Task<IEnumerable<Doctor>> GetAll()
    {
        var doctors = _clinicContext.Doctors;
        if (doctors.Count() == 0)
        {
            throw new Exception("No doctors in the database.");
        }
        return (await doctors.ToListAsync());

    }
}