// public class Doctor
// {
//     public int Id { get; set; }
//     public string Name { get; set; } = string.Empty;
//     public string Status { get; set; } = string.Empty;
//     public float YearsOfExperience { get; set; }
//     public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
//     public ICollection<Appointment>? Appointments { get; set; }
// }
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// public class DoctorSpeciality
// {
//     // [Key]
//     public int SerialNumber { get; set; }
//     public int DoctorId { get; set; }
//     public int SpecialityId { get; set; }
    
//     // [ForeignKey("SpecialityId")]
//     public Speciality? Speciality { get; set; }
//     // [ForeignKey("DoctorId")]
//     public Doctor? Doctor { get; set; }
// }
// public class Speciality
// {
//     public int Id { get; set; }
//     public string Name { get; set; } = string.Empty;
//     public string Status { get; set; } = string.Empty;
//     public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
// }
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// public class Appointment
// {
//     // [Key]
//     public int AppointmentNumber { get; set; }
//     public int PatientId { get; set; }
//     public int DoctorId { get; set; }
//     public DateTime AppointmentDate { get; set; }
//     public String Status { get; set; } = string.Empty;

//     // [ForeignKey("DoctorId")]
//     public Doctor? Doctor { get; set; }

//     // [ForeignKey("PatientId")]
//     public Patient? Patient { get; set; }
// }
// public class Patient
// {
//     public int Id { get; set; }
//     public string Name { get; set; } = string.Empty;
//     public int Age { get; set; }
//     public string Email { get; set; } = string.Empty;
//     public string Phone { get; set; } = string.Empty;
//     public string Status { get; set; } = string.Empty;
//     public ICollection<Appointment>? Appointments { get; set; }
// }
// public interface IRepository<K, T> where T : class
// {
//     public Task<T> Add(T item);
//     public Task<T> Get(K key);
//     public Task<IEnumerable<T>> GetAll();
//     public Task<T> Update(K key,T item);
//     public Task<T> Delete(K key);
// }


// public abstract class Repository<K, T> : IRepository<K, T> where T : class
// {
//     protected readonly ClinicContext _clinicContext;
    
//     public Repository(ClinicContext clinicContext)
//     {
//         _clinicContext = clinicContext;
//     }

//     public abstract Task<T> Get(K key);

//     public abstract Task<IEnumerable<T>> GetAll();

//     public async Task<T> Add(T item)
//     {
//         _clinicContext.Add(item);
//         //generate and execute the DML quries for the objects whse state is in ['added','modified','deleted'],
//         await _clinicContext.SaveChangesAsync();
//         return item;
//     }

//     public async Task<T> Update(K key, T item)
//     {
//         var myItem = await Get(key);
//         if (myItem != null)
//         {
//             _clinicContext.Entry(myItem).CurrentValues.SetValues(item);
//             await _clinicContext.SaveChangesAsync();
//             return item;
//         }
//         throw new Exception("No such item found for updation.");
//     }

//     public async Task<T> Delete(K key)
//     {
//         var item = await Get(key);
//         if (item != null)
//         {
//             _clinicContext.Remove(item);
//             await _clinicContext.SaveChangesAsync();
//             return item;
//         }
//         throw new Exception("No such item found for deleting.");
//     }
// }

// using Microsoft.EntityFrameworkCore;

// public class DoctorRepository : Repository<int, Doctor>
// {
//     public DoctorRepository(ClinicContext clinicContext) : base(clinicContext)
//     {
        
//     }
//     public override async Task<Doctor> Get(int key)
//     {
//         var doctor = await _clinicContext.Doctors.SingleOrDefaultAsync(d => d.Id == key);
//         if (doctor != null)
//         {
//             return doctor;
//         }
//         throw new Exception("Doctor not found with the given ID.");
//     }

//     public override async Task<IEnumerable<Doctor>> GetAll()
//     {
//         var doctors = _clinicContext.Doctors;
//         if (doctors.Count() == 0)
//         {
//             throw new Exception("No doctors in the database.");
//         }
//         return (await doctors.ToListAsync());

//     }
// }
// public interface IDoctorService
// {
//     public Task<Doctor> GetDoctorByName(string name);
//     public Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality);
//     public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
// }
// public class DoctorAddRequestDto
// {
//     public string Name { get; set; } = string.Empty;
//     public ICollection<SpecialityAddRequestDto>? Specialities { get; set; }
//     public float YearsOfExperience { get; set; }
// }
// public class SpecialityAddRequestDto
// {
//     public string Name { get; set; } = string.Empty;
// }