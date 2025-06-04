using AutoMapper;

public class AppointmentProfile:Profile
{
    public AppointmentProfile()
    {
        CreateMap<AppointmentAddRequestDto, Appointment>();
    }
}