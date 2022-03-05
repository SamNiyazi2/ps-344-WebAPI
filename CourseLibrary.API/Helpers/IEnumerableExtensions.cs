using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// 03/05/2022 02:18 pm - SSN - [20220305-1415] - [001] - M04-03 - Demo - Creating a reusable extension method to shape data

namespace CourseLibrary.API.Helpers
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"ps-344-webAPI-20220305-1420: Null [{nameof(source)}]");
            }


            var propertyInfoList = new List<PropertyInfo>();

            var expandoObjectList = new List<ExpandoObject>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (string field in fieldsAfterSplit)
                {
                    string propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"ps-344-webAPI-20220305-1435: Property {propertyName} was not found on {typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);

                }
            }

            foreach (TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(dataShapedObject);

            }

            return expandoObjectList;

        }
    }
}
