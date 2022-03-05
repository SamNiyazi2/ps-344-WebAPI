using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/05/2022 07:19 am - SSN - [20220305-0715] - [001] - M03-05 - Creating a property mapping service

namespace CourseLibrary.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> authorPropertyMapping = new Dictionary<string, PropertyMappingValue>
       {
           { "Id",PropertyMappingValue.Create(new[] {"id"}) },
           { "MainCategory",PropertyMappingValue.Create(new[] {"MainCategory"}) },
           { "Age",PropertyMappingValue.Create(new[] {"DateOfBirth" }, true) },
           {  "Name",PropertyMappingValue.Create(new[]{"LastName","FirstName"}) }
       };


        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(authorPropertyMapping));
        }

        // 03/05/2022 09:14 am - SSN - [20220305-0715] - [005] - M03-05 - Creating a property mapping service
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"ps-344-webAPI-20220305-0937: Cannot find mapping instance for [{typeof(PropertyMapping<TSource, TDestination>)}]]");
        }


    }
}
