using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentsManager.Models
{
    public class Appointment : IComparable<Appointment> , IEquatable<Appointment>
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }

        public Appointment() {
            PatientName = string.Empty;
            Reason = string.Empty;
        }
        public Appointment(int id, string patientName, int patientAge, DateTime appointmentDate, string reason)
        {
            Id = id;
            PatientName = patientName;
            PatientAge = patientAge;
            AppointmentDate = appointmentDate;
            Reason = reason;
        }
        public void TakeAppointmentDetailsFromUser()
        {
            Console.Write("Please enter the Patient name : ");
            string name = Console.ReadLine() ?? ""; // Null colascing
            PatientName = name;
            Console.Write("Please enter the Patient age : ");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age))
            {
                Console.WriteLine("Invalid entry for age. Please enter a valid age.");
            }
            PatientAge = age;
            Console.Write("Please enter the Appointment Date (yyyy-mm-dd) : ");
            DateTime date;
            while (!DateTime.TryParse(Console.ReadLine(), out date) || date < DateTime.Today)
            {
                Console.WriteLine("Invalid entry for appointment date. Please enter a valid appointment date.");
            }
            AppointmentDate = date;

            Console.Write("Please enter the reason for appointment : ");
            Reason = Console.ReadLine() ?? string.Empty;
        } 

        public override string ToString()
        {
            return $"ID: {Id}, Name: {PatientName}, Age: {PatientAge}, Date: {AppointmentDate}, Reason: {Reason}";
        }

        public int CompareTo(Appointment? other)
        {
            return this.Id.CompareTo(other?.Id);
        }

        public bool Equals(Appointment? other)
        {
            return this.Id == other?.Id;
        }
    }
}
