
using Microsoft.EntityFrameworkCore;

public class SpecialityRepository : Repository<int, Speciality>
{
    public SpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
    {
        
    }
    public override async Task<Speciality> Get(int key)
    {
        var speciality = await _clinicContext.Specialities.SingleOrDefaultAsync(s => s.Id == key);
        return speciality ?? throw new Exception("No specialization with the given ID.");
    }

    public override async Task<IEnumerable<Speciality>> GetAll()
    {
        var specialities = _clinicContext.Specialities;
        // if (specialities.Count() == 0)
        // {
        //     throw new Exception("No specializations in the database.");
        // }
        return (await specialities.ToListAsync());
    }
}