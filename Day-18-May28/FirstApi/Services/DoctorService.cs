

public class DoctorService : IDoctorService
{
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialityRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
    public DoctorService(IRepository<int, Doctor> doctorRepository,
                        IRepository<int, Speciality> specialityRepository,
                        IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
    {
        _doctorRepository = doctorRepository;
        _specialityRepository = specialityRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
    }

    public async Task<Doctor> GetDoctorByName(string name)
    {
        var doctors = await _doctorRepository.GetAll();
        var doctorByName = doctors.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (doctorByName == null)
        {
            throw new Exception("Doctor not found with the given name.");
        }
        return doctorByName;
    }

    public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality)
    {
        var specialities = await _specialityRepository.GetAll();
        var specialitySearch = specialities.FirstOrDefault(s => s.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase));

        if (specialitySearch == null)
        {
            throw new Exception("Speciality not found.");
        }

        var doctorSpecialities = await _doctorSpecialityRepository.GetAll();
        var doctorIds = doctorSpecialities
                        .Where(ds => ds.SpecialityId == specialitySearch.Id)
                        .Select(ds => ds.DoctorId)
                        .ToList();
        var doctors = await _doctorRepository.GetAll();
        var doctorsBySpeciality = doctors.Where(d => doctorIds.Contains(d.Id)).ToList();

        return doctorsBySpeciality;

    }

    public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctorDto)
    {
        try
        {
            var newDoctor = new Doctor
        {
            Name = doctorDto.Name,
            YearsOfExperience = doctorDto.YearsOfExperience,
            Status = "Active"
        };

        var addedDoctor = await _doctorRepository.Add(newDoctor);

        if (doctorDto.Specialities != null)
        {
            foreach (var specialityDto in doctorDto.Specialities)
            {
                var inputSpeciality = (await _specialityRepository.GetAll())
                                        .FirstOrDefault(s => s.Name.Equals(specialityDto.Name, StringComparison.OrdinalIgnoreCase));
                // if (speciality != null)
                // {
                //     var doctorSpeciality = new DoctorSpeciality
                //     {
                //         DoctorId = addedDoctor.Id,
                //         SpecialityId = speciality.Id
                //     };

                //     await _doctorSpecialityRepository.Add(doctorSpeciality);
                // }
                // else
                // {
                //     throw new Exception($"Speciality '{specialityDto.Name}' not found.");
                // }
                if (inputSpeciality == null)
                {
                    var speciality = new Speciality
                    {
                        Name = specialityDto.Name,
                        Status = "Active"
                    };
                    await _specialityRepository.Add(speciality);
                    var doctorSpeciality = new DoctorSpeciality
                    {
                        DoctorId = addedDoctor.Id,
                        SpecialityId = speciality.Id
                    };

                    await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
                else
                {
                    var doctorSpeciality = new DoctorSpeciality
                    {
                        DoctorId = addedDoctor.Id,
                        SpecialityId = inputSpeciality.Id
                    };

                    await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
                
            }
        }

        return addedDoctor;
        }
        catch (Exception e)
        {
            
            throw new Exception($"Error : {e.Message}");
        }
    }

    public async Task<ICollection<Doctor>> GetAllDoctors()
    {
        var doctors = await _doctorRepository.GetAll();
        return [.. doctors];
    }
}