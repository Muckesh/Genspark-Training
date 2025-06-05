using AutoMapper;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<EmployeeAddRequestDto, User>()
        .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Email))
        .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<EmployeeAddRequestDto, Employee>();
    }
}