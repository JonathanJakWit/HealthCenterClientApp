using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Models
{
    public class Doctor
    {
        public int EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public int? SpecializationId { get; set; }
        public string PhoneNumber { get; set; }
        public Specialization Specialization { get; set; }
        public string Status { get; set; } = "ACTIVE";

        public override string ToString()
        {
            return $"EmployeeNR: {EmployeeNumber} | Name: {FirstName} {LastName} | Specialization: {Specialization.SpecializationName} | PhoneNR: {PhoneNumber} | Status: {Status}";
        }
    }
}
