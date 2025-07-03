
public class OtherFunctionalityImplementation : IOtherContextFunctionalities
{
    private readonly ClinicContext _clinicContext;

    public OtherFunctionalityImplementation(ClinicContext clinicContext)
    {
        _clinicContext = clinicContext;
        
    }
    public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
    {
        var result = await _clinicContext.GetDoctorsBySpeciality(speciality);
        return result;
    }
}