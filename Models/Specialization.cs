using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public decimal VisitCost { get; set; }

        public override string ToString()
        {
            return $"{SpecializationId},  {SpecializationName}, {VisitCost}";
        }
    }
}
