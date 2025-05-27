public interface IAppointmentService
{
    IEnumerable<Appointment> GetAppointments();
    Appointment GetAppointment(int id);
    int AddAppointment(Appointment appointment);
    int UpdateAppointment(Appointment appointment);
    int DeleteAppointment(int id);
}