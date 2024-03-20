using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCenterClientApp.Models
{
    public enum WeekDays
    {
        Monday, Tuesday, Wednesday, Thursday, Friday
    }
    public enum TimeSlots
    {
        Nine, NineThirty,
        Ten, TenThirty
    }

    public class AvailabilitySlot
    {
        public WeekDays WeekDay { get; set; }
        public TimeSlots TimeSlot { get; set; }
    }
}
