using System;
using System.Collections.Concurrent;
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








        // 03/06/2022 05:21 pm - SSN - [20220305-1512] - [005] - M04-05 - Demo - Data shaping single resources
        static ConcurrentDictionary<Type, PropertyInfo[]> sourceData = new System.Collections.Concurrent.ConcurrentDictionary<Type, PropertyInfo[]>();

        public static IEnumerable<ExpandoObject> ShapeData_v2<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"ps-344-webAPI-20220305-1420: Null [{nameof(source)}]");
            }


            if (!sourceData.TryGetValue(source.GetType(), out PropertyInfo[] propertyInfos))
            {
                propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                sourceData.TryAdd(source.GetType(), propertyInfos);

            }


            string[] fieldsAfterSplit = { };

            if (!string.IsNullOrWhiteSpace(fields))
            {

                fieldsAfterSplit = fields.Split(',');

                foreach (string fieldName in fieldsAfterSplit)
                {
                    if (!propertyInfos.Any(r => r.Name.ToLower() == fieldName.Trim().ToLower()))
                    {
                        throw new Exception($"ps-344-webAPI-20220306-1731: Property {fieldName} was not found on {typeof(TSource)}");
                    }
                }

            }


            var expandoObjectList = new List<ExpandoObject>();

            foreach (TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach (var propertyInfo in propertyInfos.Where(r => string.IsNullOrWhiteSpace(fields) || fieldsAfterSplit.Any(r2 =>
               r2.ToString().ToLower().Trim() == r.Name.ToLower())))
                {
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyInfo.GetValue(sourceObject));
                }

                expandoObjectList.Add(dataShapedObject);

            }

            return expandoObjectList;

        }
    }
}
