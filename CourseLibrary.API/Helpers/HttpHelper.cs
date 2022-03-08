using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/07/2022 12:53 pm - SSN - [20220307-1105] - [007] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types
// https://stackoverflow.com/questions/61914021/how-can-we-access-request-context-in-custom-attributes-in-asp-net-core

namespace CourseLibrary.API.Helpers
{
    public static class HttpHelper
    {
        private static IHttpContextAccessor httpContextAccessor;

        public static void Configure(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
        }

        public static HttpContext httpContext => httpContextAccessor?.HttpContext;
    }
}
