using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentsManager.Interfaces;
using AppointmentsManager.Models;

namespace AppointmentsManager.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<int, Appointment> _appointmentRepository;

        public AppointmentService(IRepository<int, Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public int AddAppointment(Appointment appointment)
        {
            try
            {
                var result = _appointmentRepository.Add(appointment);
                if (result != null)
                {
                    return result.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}.");
            }
            return -1;
        }

        public List<Appointment>? SearchAppointment(AppointmentSearchModel appointmentSearchModel)
        {
            try
            {
                var appointments = _appointmentRepository.GetAll();
                appointments = SearchById(appointments, appointmentSearchModel.Id);
                appointments = SearchByPatientName(appointments, appointmentSearchModel.PatientName);
                appointments = SearchByAge(appointments, appointmentSearchModel.AgeRange);
                appointments = SearchByDate(appointments, appointmentSearchModel.AppointmentDate);
                if (appointments != null && appointments.Count > 0)
                    return appointments.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}.");
            }
            return null;
        }

        private ICollection<Appointment> SearchById(ICollection<Appointment> appointments, int? id)
        {
            if (id == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.Id == id).ToList();
            }
        }

        private ICollection<Appointment> SearchByPatientName(ICollection<Appointment> appointments, string? patientName)
        {
            if (string.IsNullOrWhiteSpace(patientName) || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.PatientName.ToLower().Contains(patientName.ToLower())).ToList();
            }
        }

        private ICollection<Appointment> SearchByAge(ICollection<Appointment> appointments, Range<int>? ageRange)
        {
            if (ageRange == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.PatientAge >= ageRange.MinVal && a.PatientAge <= ageRange.MaxVal).ToList();
            }
        }

        private ICollection<Appointment> SearchByDate(ICollection<Appointment> appointments, Range<DateTime>? dateRange)
        {
            if (dateRange == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.AppointmentDate >= dateRange.MinVal && a.AppointmentDate <= dateRange.MaxVal).ToList();
            }
        }
    }
}
