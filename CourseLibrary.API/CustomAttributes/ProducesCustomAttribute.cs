using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 03/07/2022 12:02 pm - SSN - [20220307-1105] - [005] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types

namespace CourseLibrary.API.CustomAttributes
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class ProducesAttribute : System.Attribute
    {

        public ProducesAttribute(params string[] mediaSubTypeWithoutSuffix)
        {

            //var headerEntry = Helpers.HttpHelper.httpContext?.Response.Headers.FirstOrDefault(r => r.Key == "Content-Type");
            //if (headerEntry?.Key == null)
            //{
            //    List<string> mediaTypes = new List<string>();

            //    for (int x = 0; x < mediaSubTypeWithoutSuffix.Count(); x++)
            //    {
            //        mediaTypes.Add($"application/{ mediaSubTypeWithoutSuffix[x] }+json"); 
            //    }
            //    Helpers.HttpHelper.httpContext?.Response.Headers.Add("Content-Type", mediaTypes.ToArray());
            //}


        }
    }
}
