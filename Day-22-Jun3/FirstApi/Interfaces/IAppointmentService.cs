public interface IAppointmentService
{
    public Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment);
    public Task<ICollection<Appointment>> GetAllAppointments();
    public Task<Appointment> CancelAppointment(int appointmentNumber);
}