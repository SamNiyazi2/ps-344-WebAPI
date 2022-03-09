using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// 03/06/2022 10:16 pm - SSN - [20220306-2206] - [002] - M06-04 - Demo - HATEOAS and content negotiation

namespace CourseLibrary.API.Helpers
{
    public class Constants
    {
        public const string MEDIA_TYPE_APPLICATION_JSON = "application/json";
        public const string MEDIA_TYPE_APPLICATION_VND_MARGIN_HATEOAS_JSON = "application/vnd.marvin.hateoas+json";
        public const string MEDIA_TYPE_APPLICATION_VND_MARVIN_AUTHOR_FULL_JSON = "application/vnd.marvin.author.full+json";
        public const string MEDIA_TYPE_APPLICATION_VND_MARVIN_AUTHOR_FULL_HATEOAS_JSON = "application/vnd.marvin.author.full.hateoas+json";
        public const string MEDIA_TYPE_APPLICATION_VND_MARVIN_AUTHOR_FRIENDLY_JSON = "application/vnd.marvin.author.friendly+json";
        public const string MEDIA_TYPE_APPLICATION_VND_MARVIN_AUTHOR_FRIENDLY_HATEOAS_JSON = "application/vnd.marvin.author.friendly.hateoas+json";


        // 03/08/2022 03:42 pm - SSN - [20220307-2140] - [008] - M06-08 - Demo - Working with vendor-specific media types on input
        public const string MEDIA_TYPE_APPLICATION_VND_MARVIN_AUTHOR_FOR_CREATION_JSON = "application/vnd.marvin.authorforcreation+json";
        public const string MEDIA_TYPE_APPLICATION_VND_MARVIN_AUTHOR_FOR_CREATION_WITH_DATEOFDEATH_JSON = "application/vnd.marvin.authorforcreationwithdateofdeath+json";

    }
}
