using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/05/2022 09:30 am - SSN - [20220305-0715] - [006] - M03-05 - Creating a property mapping service

namespace CourseLibrary.API.Services
{
    public interface IPropertyMapping
    {
        Dictionary<string, PropertyMappingValue> MappingDictionary { get; }
    }
}
