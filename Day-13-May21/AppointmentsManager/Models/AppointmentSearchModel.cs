using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentsManager.Models
{
    public class AppointmentSearchModel
    {
        public int? Id { get; set; }
        public string? PatientName { get; set; }
        public Range<DateTime>? AppointmentDate { get; set; }
        public Range<int>? AgeRange { get; set; }
    }

    public class Range<T>
    {
        public T? MinVal { get; set; }
        public T? MaxVal { get; set; }
    }
}
