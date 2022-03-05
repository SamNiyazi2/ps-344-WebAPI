using CourseLibrary.API.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/05/2022 07:58 am - SSN - [20220305-0715] - [003] - M03-05 - Creating a property mapping service

using System.Linq.Dynamic.Core;

namespace CourseLibrary.API.Helpers
{
    public static class IQueryableExtensions
    {
        const string error_prefix = "ps-344-webApi-ApplySort-20220305";

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"{error_prefix}-0801: Null {nameof(source)}");
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException($"{error_prefix}-0802: Null {nameof(mappingDictionary)}");
            }

            if (string.IsNullOrEmpty(orderBy))
            {
                return source;
            }

            var orderByAfterSplit = orderBy.Split(',');
            string orderByString = "";

            foreach (var orderByClause in orderByAfterSplit)
            {
                var trimmedOrderByClause = orderByClause.Trim();
                var orderDescending = trimmedOrderByClause.ToLower().EndsWith(" desc");

                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                var mappingEntry = mappingDictionary.FirstOrDefault(r => r.Key.ToLower() == propertyName.ToLower());

                if (mappingEntry.Key == null)
                {
                    throw new ArgumentNullException($"{error_prefix}-0812: No order mapping for {propertyName} ");
                }

                var propertyMappingValue = mappingEntry.Value;

                if (!propertyMappingValue.EntryValidated)
                {

                    if (propertyMappingValue == null)
                    {
                        throw new ArgumentNullException($"{error_prefix}-0816: Mapping for {propertyName} has no definition");
                    }

                    if (propertyMappingValue.DestinationProperties == null)
                    {
                        throw new ArgumentNullException($"{error_prefix}-1220: Mapping for {propertyName} is null for DestinationProperties.");
                    }

                    if (propertyMappingValue.DestinationProperties.Count() == 0)
                    {
                        throw new ArgumentNullException($"{error_prefix}-1225: Mapping for {propertyName} DestinationProperties has no entries.");
                    }

                    propertyMappingValue.EntryValidated = true;
                }


                orderDescending = propertyMappingValue.Revert ? !orderDescending : orderDescending;


                foreach (var desinationProperty in propertyMappingValue.DestinationProperties)
                {
                    orderByString += (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ") +
                        desinationProperty +
                        (orderDescending ? " descending" : string.Empty);
                }

            }

            return source.OrderBy(orderByString);


        }
    }
}
