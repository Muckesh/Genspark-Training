using AppointmentsManager;
using AppointmentsManager.Interfaces;
using AppointmentsManager.Models;
using AppointmentsManager.Repositories;
using AppointmentsManager.Services;

class Program
{
    static void Main(string[] args)
    {
        IRepository<int, Appointment> repo = new AppointmentRepository();
        IAppointmentService service = new AppointmentService(repo);
        ManageAppointment manager = new ManageAppointment(service);
        manager.Start();
    }
}
