using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/05/2022 09:06 am - SSN - [20220305-0715] - [004] - M03-05 - Creating a property mapping service

namespace CourseLibrary.API.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; }


        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ?? throw new NullReferenceException($"ps-344-webApi-20220305-0909: Null [{nameof(mappingDictionary)}]");
        }
    }
}
