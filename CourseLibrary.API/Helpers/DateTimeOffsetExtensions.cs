using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        // 03/07/2022 09:45 pm - SSN - [20220307-2140] - [002] - M06-08 - Demo - Working with vendor-specific media types on input
        // Add DateOfDeath
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset, DateTimeOffset? dateOfDeath)
        {
            var currentDate = dateOfDeath.HasValue ? dateOfDeath.Value.UtcDateTime : DateTime.UtcNow;

            int age = currentDate.Year - dateTimeOffset.Year;

            if (currentDate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
