
using AutoMapper;

public class PatientService : IPatientService
{
    private readonly IRepository<int, Patient> _patientRepository;
    private readonly IRepository<string, User> _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IMapper _mapper;

    public PatientService(IRepository<int, Patient> patientRepository,
                            IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IMapper mapper)
    {
        _patientRepository = patientRepository;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _mapper = mapper;
    }
    public async Task<Patient> AddPatient(PatientAddRequestDto requestDto)
    {
        try
        {
            var user = _mapper.Map<PatientAddRequestDto, User>(requestDto);
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = requestDto.Password
            });
            user.Password = encryptedData.EncryptedData;
            user.Role = "Patient";
            user.HashKey = encryptedData.HashKey;

            await _userRepository.Add(user);

            var patient = _mapper.Map<PatientAddRequestDto, Patient>(requestDto);
            patient = await _patientRepository.Add(patient);
            return patient;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    public async Task<ICollection<Patient>> GetAllPatients()
    {
        var doctors = await _patientRepository.GetAll();
        return doctors.ToList();
    }
}