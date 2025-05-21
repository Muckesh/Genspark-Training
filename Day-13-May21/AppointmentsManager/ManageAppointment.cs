using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AppointmentsManager.Interfaces;
using AppointmentsManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentsManager
{
    public class ManageAppointment
    {
        private readonly IAppointmentService _appointmentService;

        public ManageAppointment(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddAppointment();
                        break;
                    case "2":
                        SearchAppointments();
                        break;
                    case "3": exit = true; break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        public void PrintMenu()
        {
            Console.WriteLine("------------------------ Appointment Management Menu ---------------------------------");
            Console.WriteLine("1. Add Appointment");
            Console.WriteLine("2. Search Appointment");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice : ");
        }

        public void AddAppointment()
        {
            Appointment appointment = new Appointment();
            appointment.TakeAppointmentDetailsFromUser();
            int id = _appointmentService.AddAppointment(appointment);
            if (id != -1)
            {
                Console.WriteLine($"Appointment added successfully. ID: {id}");
            }
            else
            {
                Console.WriteLine("Failed to add appointment.");
            }
        }

        public void SearchAppointments()
        {
            AppointmentSearchModel searchModel = new AppointmentSearchModel();

            Console.Write("Search by Patient ID (leave blank to skip): ");
            int id;
            if (int.TryParse(Console.ReadLine(), out id))
                searchModel.Id = id;

            Console.Write("Search by Patient Name (leave blank to skip): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                searchModel.PatientName = name;

            Console.Write("Search by Age Range (e.g., 30-50, leave blank to skip): ");
            var ageInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ageInput) && ageInput.Contains('-'))
            {
                var parts = ageInput.Split('-');
                if (int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
                {
                    searchModel.AgeRange = new Range<int> { MinVal = min, MaxVal = max };
                }
            }

            Console.Write("Enter start date to search yyyy-mm-dd (or leave blank): ");
            var startDateInput = Console.ReadLine();
            Console.Write("Enter end date yyyy-mm-dd (or leave blank): ");
            var endDateInput = Console.ReadLine();
            if (DateTime.TryParse(startDateInput, out DateTime startDate) || DateTime.TryParse(endDateInput, out DateTime endDate))
            {
                searchModel.AppointmentDate = new Range<DateTime>
                {
                    MinVal = DateTime.TryParse(startDateInput, out startDate) ? startDate : DateTime.MinValue,
                    MaxVal = DateTime.TryParse(endDateInput, out endDate) ? endDate : DateTime.MaxValue
                };
            }
            
            var results = _appointmentService.SearchAppointment(searchModel);
            if (results != null && results.Count > 0)
            {
                Console.WriteLine("\n--- Matching Appointments ---");
                foreach (var a in results)
                {
                    Console.WriteLine(a.ToString());
                }
            }
            else
            {
                Console.WriteLine("No appointments found.");
            }
        }
    }
}

