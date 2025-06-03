
using AutoMapper;

public class AppointmentService:IAppointmentService
{
    private readonly IRepository<int, Appointment> _appointmentRepository;
    private readonly IMapper _mapper;

    public AppointmentService(IRepository<int, Appointment> appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment)
    {
        try
        {
            var newAppointment = _mapper.Map<AppointmentAddRequestDto, Appointment>(appointment);
            newAppointment.Status = "Scheduled";
            newAppointment = await _appointmentRepository.Add(newAppointment);
            return newAppointment;
        }
        catch(Exception e)
        {
            throw new Exception(e.Message);
        }

    }

    public async Task<Appointment> CancelAppointment(int appointmentNumber)
    {
        try
        {
            var appointment = await _appointmentRepository.Get(appointmentNumber);
            if (appointment == null)
                throw new Exception("Appointment not found.");
            appointment.Status = "Cancelled";
            appointment = await _appointmentRepository.Update(appointmentNumber, appointment);
            return appointment;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<ICollection<Appointment>> GetAllAppointments()
    {
        try
        {
            var appointments = await _appointmentRepository.GetAll();
            if (appointments == null)
                throw new Exception("No appointments in the database.");
            return appointments.ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}