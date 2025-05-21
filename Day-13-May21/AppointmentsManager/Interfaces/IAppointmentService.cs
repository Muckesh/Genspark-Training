using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentsManager.Models;

namespace AppointmentsManager.Interfaces
{
    public interface IAppointmentService
    {
        int AddAppointment(Appointment appointment);

        List<Appointment>? SearchAppointment(AppointmentSearchModel appointmentSearchModel);
    }
}
