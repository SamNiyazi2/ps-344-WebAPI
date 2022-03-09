using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/08/2022 02:47 pm - SSN - [20220307-2140] - [006] - M06-08 - Demo - Working with vendor-specific media types on input

namespace CourseLibrary.API.ActionConstraints
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
    {
        private readonly string requestHeaderToatch;
        private readonly MediaTypeCollection mediaTypes = new MediaTypeCollection();

        public RequestHeaderMatchesMediaTypeAttribute(string _requestHeaderToatch, params string[] _mediaTypes)
        {
            this.requestHeaderToatch = _requestHeaderToatch??throw new ArgumentNullException($"ps-344-webAPI-20220308-1458: Null [{nameof(_requestHeaderToatch)}]");
            
            foreach ( string mediaType in _mediaTypes)
            {
                if( MediaTypeHeaderValue.TryParse( mediaType, out MediaTypeHeaderValue parsedMediaType))
                {
                    mediaTypes.Add(mediaType);
                }
                else
                {
                    throw new ArgumentException($"ps-344-webAPI-20220308-1501: Failed to parse media type [{mediaType}]");
                }
            }

            if (mediaTypes.Count == 0)
            {
                throw new ArgumentException($"ps-344-webAPI-20220308-1504: Must pass at least one media type.");
            }

        }


        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;
            if ( !requestHeaders.ContainsKey(requestHeaderToatch)) 
            {
                return false;
            }

            var parsedRequestMediaType = new MediaType(requestHeaders[requestHeaderToatch]);

            foreach ( var mediaType in mediaTypes)
            {
                string currentAction = context.RouteContext.RouteData.Values["Action"]?.ToString();
                string currentCandidateAction = context.CurrentCandidate.Action.RouteValues["Action"]?.ToString();
                if (currentAction == currentCandidateAction)
                {
                    if (parsedRequestMediaType.Equals(new MediaType(mediaType))) 
                    return true;
                }
            }

            return false;

        }

    }
}
