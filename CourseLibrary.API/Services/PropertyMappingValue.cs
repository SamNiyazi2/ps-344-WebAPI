using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/05/2022 07:21 am - SSN - [20220305-0715] - [002] - M03-05 - Creating a property mapping service

namespace CourseLibrary.API.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; }
        public bool Revert { get; }
        public bool EntryValidated { get; set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties ?? throw new ArgumentNullException($"ps-344-webApi-20220305-0727: Null [{nameof(destinationProperties)}]");
            Revert = revert;
        }

        internal static PropertyMappingValue Create(string[] destinationProperties, bool revert = false)
        {
            return new PropertyMappingValue(destinationProperties, revert);
        }

    }
}
