// public class AppointmentRepository : Repository<int, Appointment>
// {
//     public AppointmentRepository() : base()
//     {

//     }

//     public override ICollection<Appointment> GetAll()
//     {
//         if (_items.Count == 0)
//         {
//             throw new KeyNotFoundException("No Appointments found.");
//         }
//         return _items;
//     }

//     public override Appointment GetById(int id)
//     {
//         var appointment = _items.FirstOrDefault(a => a.Id == id);
//         if (appointment == null)
//         {
//             throw new KeyNotFoundException("Appointment not found.");
//         }
//         return appointment;
//     }

//     public override Appointment Update(Appointment appointment)
//     {
//         var existingAppointment = GetById(appointment.Id);
//         if (existingAppointment == null)
//         {
//             throw new KeyNotFoundException("Appointment not found.");
//         }
//         existingAppointment.DoctorId = appointment.DoctorId;
//         existingAppointment.PatientId = appointment.PatientId;
//         existingAppointment.AppointmentDate = appointment.AppointmentDate;
//         existingAppointment.AppointmentStatus = appointment.AppointmentStatus;
//         return existingAppointment;
//     }

//     protected override int GenerateId()
//     {
//         if (_items.Count == 0)
//         {
//             return 101;
//         }
//         else
//         {
//             return _items.Max(a => a.Id) + 1;
//         }
//     }
// }