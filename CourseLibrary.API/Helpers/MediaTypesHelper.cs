using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

// 03/07/2022 05:07 pm - SSN - [20220307-1105] - [011] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types

namespace CourseLibrary.API.Helpers
{
    public static class MediaTypesHelper
    {
        static ConcurrentDictionary<string, MediaTypeHeaderValue> mediaTypesHeaderValues = new ConcurrentDictionary<string, MediaTypeHeaderValue>();

        public static string SubTypeWithoutSuffix_STRING(this string mediaTypeString)
        {
            MediaTypeHeaderValue temp = mediaTypesHeaderValues.GetOrAdd(mediaTypeString, new MediaTypeHeaderValue(mediaTypeString));
            return temp.SubTypeWithoutSuffix.ToString();

        }
    }
}
