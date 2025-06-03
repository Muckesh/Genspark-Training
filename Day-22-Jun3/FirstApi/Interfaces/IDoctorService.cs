public interface IDoctorService
{
    public Task<Doctor> GetDoctorByName(string name);
    public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality);
    public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
    public Task<ICollection<Doctor>> GetAllDoctors();
}