using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Models
{
    public class Patient
    {
        public int PatientMedicalNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Street { get; set; }
        public string PostZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public SqlDateTime Birthday { get; set; }
        public SqlDateTime RegisterDate { get; set; }

        public override string ToString()
        {
            return $"MedicalNR: {PatientMedicalNumber} | Name: {FirstName} {LastName} | Street: {Street} | PhoneNR: {PhoneNumber} | Birthday: {Birthday.ToString()} | RegisterDate: {RegisterDate.ToString()}";
        }
    }
}
