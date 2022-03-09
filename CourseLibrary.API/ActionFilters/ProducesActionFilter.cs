using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// 03/07/2022 02:52 pm - SSN - [20220307-1105] - [009] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types

namespace CourseLibrary.API.ActionFilters
{
    public class ProducesActionFilter : Attribute, IActionFilter
    {
        List<MediaTypeHeaderValue> mediaTypes = new List<MediaTypeHeaderValue>();

        public ProducesActionFilter(params string[] mediaSubTypeWithoutSuffix)
        {
            foreach (string mediaType in mediaSubTypeWithoutSuffix)
            {
                mediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var acceptHeaderEntry = Helpers.HttpHelper.httpContext?.Request.Headers.FirstOrDefault(r => r.Key == "Accept");
            var acceptHeaderEntry = context.HttpContext.Request.Headers.FirstOrDefault(r => r.Key == "Accept");

            // MediaTypeHeaderValue contentType = mediaTypes.FirstOrDefault(r => r.MediaType == acceptHeaderEntry.Value.Value);
            MediaTypeHeaderValue contentType = mediaTypes.FirstOrDefault(r => r.MediaType == acceptHeaderEntry.Value);

            if (contentType == null) return;

            // New response. No need to check if entry exists.
            // Helpers.HttpHelper.httpContext?.Response.Headers.Add("Content-Type", contentType.MediaType);
            context.HttpContext.Response.Headers.Add("Content-Type", contentType.MediaType);

        }
    }
}
