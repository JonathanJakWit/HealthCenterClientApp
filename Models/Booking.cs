using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public SqlDateTime AppointmentDate { get; set; }
        public string MedicalNotes { get; set; }
        public int DoctorEmployeeNumber { get; set; }
        public int PatientMedicalNumber { get; set; }

        public override string ToString()
        {
            return $"BookingId: {BookingId} | AppointmentDate: {AppointmentDate.ToString()} | MedicalNotes: {MedicalNotes} | DoctorEmployeeNumber: {DoctorEmployeeNumber} | PatientMedicalNumber: {PatientMedicalNumber}";
        }
    }
}
