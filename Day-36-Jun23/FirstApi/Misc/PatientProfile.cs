using AutoMapper;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<PatientAddRequestDto, Patient>();
    }
    
}