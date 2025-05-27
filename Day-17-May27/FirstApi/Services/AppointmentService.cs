
// public class AppointmentService : IAppointmentService
// {
//     private readonly IRepository<int, Appointment> _appointmentRepository;

//     public AppointmentService(IRepository<int, Appointment> appointmentRepository)
//     {
//         _appointmentRepository = appointmentRepository;
//     }

//     public int AddAppointment(Appointment appointment)
//     {
//         try
//         {
//             var result = _appointmentRepository.Add(appointment);
//             if (result != null)
//             {
//                 return result.Id;
//             }
//         }
//         catch (Exception e)
//         {

//             Console.WriteLine($"Error : {e.Message}.");
//         }
//         return -1;
//     }

//     public int DeleteAppointment(int id)
//     {
//         _appointmentRepository.Delete(id);
//         return id;
//     }

//     public Appointment GetAppointment(int id)
//     {
//         return _appointmentRepository.GetById(id);
//     }

//     public IEnumerable<Appointment> GetAppointments()
//     {
//         return _appointmentRepository.GetAll();
//     }

//     public int UpdateAppointment(Appointment appointment)
//     {
//         _appointmentRepository.Update(appointment);
//         return appointment.Id;
//     }
// }