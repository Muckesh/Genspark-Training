public interface IPatientService
{
    public Task<Patient> AddPatient(PatientAddRequestDto requestDto);
    public Task<ICollection<Patient>> GetAllPatients();
}