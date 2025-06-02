public interface IPatientService
{
    public Task<Patient> AddPatient(PatientAddRequestDto requestDto);
}