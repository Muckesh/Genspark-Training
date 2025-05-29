

public class DoctorService : IDoctorService
{
    DoctorMapper doctorMapper;
    SpecialityMapper specialityMapper;
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialityRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
    private readonly IOtherContextFunctionalities _otherContextFunctionalities;
    public DoctorService(IRepository<int, Doctor> doctorRepository,
                        IRepository<int, Speciality> specialityRepository,
                        IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
                        IOtherContextFunctionalities otherContextFunctionalities)
    {
        doctorMapper = new DoctorMapper();
        specialityMapper = new SpecialityMapper();
        _doctorRepository = doctorRepository;
        _specialityRepository = specialityRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
        _otherContextFunctionalities = otherContextFunctionalities;
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

    public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
    {
        // var specialities = await _specialityRepository.GetAll();
        // var specialitySearch = specialities.FirstOrDefault(s => s.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase));

        // if (specialitySearch == null)
        // {
        //     throw new Exception("Speciality not found.");
        // }

        // var doctorSpecialities = await _doctorSpecialityRepository.GetAll();
        // var doctorIds = doctorSpecialities
        //                 .Where(ds => ds.SpecialityId == specialitySearch.Id)
        //                 .Select(ds => ds.DoctorId)
        //                 .ToList();
        // var doctors = await _doctorRepository.GetAll();
        // var doctorsBySpeciality = doctors.Where(d => doctorIds.Contains(d.Id)).ToList();

        // return doctorsBySpeciality;

        try
        {
            var result = await _otherContextFunctionalities.GetDoctorsBySpeciality(speciality);
            return result;
        }
        catch (Exception e)
        {

            throw new Exception(e.Message);
        }
        return null;

    }

    public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
    {
        try
        {
            var newDoctor = doctorMapper.MapDoctorAddRequestDoctor(doctor);
            newDoctor = await _doctorRepository.Add(newDoctor);
            if (newDoctor == null)
            {
                throw new Exception("Could not add doctor");
            }
            if (doctor.Specialities.Count() > 0)
            {
                int[] specialities = await MapAndAddSpeciality(doctor);
                for (int i = 0; i < specialities.Length; i++)
                {
                    var doctorSpeciality = specialityMapper.MapDoctorSpeciality(newDoctor.Id, specialities[i]);
                    doctorSpeciality = await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
            }
            return newDoctor;
        }
        catch (Exception e)
        {

            throw new Exception(e.Message);
        }
        // try
        // {
        //     var newDoctor = new Doctor
        // {
        //     Name = doctorDto.Name,
        //     YearsOfExperience = doctorDto.YearsOfExperience,
        //     Status = "Active"
        // };

        // var addedDoctor = await _doctorRepository.Add(newDoctor);

        // if (doctorDto.Specialities != null)
        // {
        //     foreach (var specialityDto in doctorDto.Specialities)
        //     {
        //         var inputSpeciality = (await _specialityRepository.GetAll())
        //                                 .FirstOrDefault(s => s.Name.Equals(specialityDto.Name, StringComparison.OrdinalIgnoreCase));
        //         // if (speciality != null)
        //         // {
        //         //     var doctorSpeciality = new DoctorSpeciality
        //         //     {
        //         //         DoctorId = addedDoctor.Id,
        //         //         SpecialityId = speciality.Id
        //         //     };

        //         //     await _doctorSpecialityRepository.Add(doctorSpeciality);
        //         // }
        //         // else
        //         // {
        //         //     throw new Exception($"Speciality '{specialityDto.Name}' not found.");
        //         // }
        //         if (inputSpeciality == null)
        //         {
        //             var speciality = new Speciality
        //             {
        //                 Name = specialityDto.Name,
        //                 Status = "Active"
        //             };
        //             await _specialityRepository.Add(speciality);
        //             var doctorSpeciality = new DoctorSpeciality
        //             {
        //                 DoctorId = addedDoctor.Id,
        //                 SpecialityId = speciality.Id
        //             };

        //             await _doctorSpecialityRepository.Add(doctorSpeciality);
        //         }
        //         else
        //         {
        //             var doctorSpeciality = new DoctorSpeciality
        //             {
        //                 DoctorId = addedDoctor.Id,
        //                 SpecialityId = inputSpeciality.Id
        //             };

        //             await _doctorSpecialityRepository.Add(doctorSpeciality);
        //         }

        //     }
        // }

        // return addedDoctor;
        // }
        // catch (Exception e)
        // {

        //     throw new Exception($"Error : {e.Message}");
        // }
    }

    private async Task<int[]> MapAndAddSpeciality(DoctorAddRequestDto doctor)
    {
        int[] specialityIds = new int[doctor.Specialities.Count()];
        IEnumerable<Speciality> existingSpecialities = null;
        try
        {
            existingSpecialities = await _specialityRepository.GetAll();
        }
        catch (Exception e)
        {

        }
        int count = 0;
        foreach (var item in doctor.Specialities)
        {
            Speciality speciality = null;
            if (existingSpecialities != null)

                speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
            if (speciality == null)
            {
                speciality = specialityMapper.MapSpecialityAddRequestDoctor(item);
                speciality = await _specialityRepository.Add(speciality);
            }
            specialityIds[count] = speciality.Id;
            count++;
        }
        return specialityIds;
    }

    public async Task<ICollection<Doctor>> GetAllDoctors()
    {
        var doctors = await _doctorRepository.GetAll();
        return [.. doctors];
    }
}