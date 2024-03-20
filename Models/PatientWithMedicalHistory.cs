using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Models
{
    public class PatientWithMedicalHistory
    {
        public int PatientMedicalNumber { get; set; }
        public List<SqlDateTime> PatientAppointmentDates { get; set; }
        public List<string> PatientMedicalNotes { get; set; }

        public override string ToString()
        {
            string finishedStr = "";
            finishedStr += "PatientMedNR: " + PatientMedicalNumber + "\n";
            for (int i = 0; i < PatientAppointmentDates.Count; i++)
            {
                finishedStr += "Date: " + PatientAppointmentDates[i] + " : " + "Medical Note: " + PatientMedicalNotes[i] + "\n";
            }

            return finishedStr;
        }
    }
}
