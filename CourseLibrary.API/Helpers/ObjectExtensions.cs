﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// 03/05/2022 03:19 pm - SSN - [20220305-1512] - [001] - M04-05 - Demo - Data shaping single resources
// Copied from downloaded code.

namespace CourseLibrary.API.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source,
             string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dataShapedObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject 
                var propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    // get the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(source);

                    // add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                return dataShapedObject;
            }

            // the field are separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                // trim each field, as it might contain leading 
                // or trailing spaces. Can't trim the var in foreach,
                // so use another var.
                var propertyName = field.Trim();

                // use reflection to get the property on the source object
                // we need to include public and instance, b/c specifying a 
                // binding flag overwrites the already-existing binding flags.
                var propertyInfo = typeof(TSource)
                    .GetProperty(propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyName} wasn't found " +
                        $"on {typeof(TSource)}");
                }

                // get the value of the property on the source object
                var propertyValue = propertyInfo.GetValue(source);

                // add the field to the ExpandoObject
                ((IDictionary<string, object>)dataShapedObject)
                    .Add(propertyInfo.Name, propertyValue);
            }

            // return the list
            return dataShapedObject;
        }



        // 03/06/2022 05:20 pm - SSN - [20220305-1512] - [004] - M04-05 - Demo - Data shaping single resources

        static ConcurrentDictionary<Type, PropertyInfo[]> sourceData = new System.Collections.Concurrent.ConcurrentDictionary<Type, PropertyInfo[]>();

        public static ExpandoObject ShapeData_v2<TSource>(this TSource source,
             string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!sourceData.TryGetValue(source.GetType(), out PropertyInfo[] propertyInfos))
            {
                propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                sourceData.TryAdd(source.GetType(), propertyInfos);

            }

            var dataShapedObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject 

                foreach (var propertyInfo in propertyInfos)
                {
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyInfo.GetValue(source));
                }

                return dataShapedObject;
            }

            // the field are separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            foreach (var propertyName in fieldsAfterSplit)
            {
                var propertyInfo = propertyInfos.FirstOrDefault(r => r.Name.ToLower() == propertyName.Trim().ToLower());

                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyName} wasn't found " +
                        $"on {typeof(TSource)}");
                }

                ((IDictionary<string, object>)dataShapedObject)
                    .Add(propertyInfo.Name, propertyInfo.GetValue(source));
            }

            return dataShapedObject;
        }

    }
}
