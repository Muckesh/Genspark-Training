public class DoctorMapper
{
    public Doctor? MapDoctorAddRequestDoctor(DoctorAddRequestDto addRequestDto)
    {
        Doctor doctor = new();
        doctor.Name = addRequestDto.Name;
        doctor.YearsOfExperience = addRequestDto.YearsOfExperience;
        return doctor;
    }
}