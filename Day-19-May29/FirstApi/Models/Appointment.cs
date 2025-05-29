using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Appointment
{
    // [Key]
    public int AppointmentNumber { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public String Status { get; set; } = string.Empty;

    // [ForeignKey("DoctorId")]
    public Doctor? Doctor { get; set; }

    // [ForeignKey("PatientId")]
    public Patient? Patient { get; set; }
}