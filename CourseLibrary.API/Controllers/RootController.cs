using CourseLibrary.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/06/2022 09:38 pm - SSN - [20220306-2135] - [001] - M05-09 - Demo - Working towards self-discoverability with a root document

namespace CourseLibrary.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {

        [HttpGet(Name ="GetRoot")]
        public IActionResult getRoot()
        {

            var links = new List<LinkDTO>();

            links.Add(new LinkDTO(Url.Link("GetRoot", new { }), "self", "GET"));
            links.Add(new LinkDTO(Url.Link("GetAuthors", new { }), "GetAuthors", "GET"));
            links.Add(new LinkDTO(Url.Link("CreateAuthor", new { }), "CreateAuthor", "POST"));

            return Ok(links);
        }
    }
}
