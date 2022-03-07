using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {

        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        // 03/04/2022 09:24 pm - SSN - [20220304-2120] - [001] - M02-10 - Demo - Returning pagination metadata

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        // 03/05/2022 02:49 pm - SSN - [20220305-1446] - [002] - M04-04 - Demo - Data shaping collection resources
        //public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
        public ActionResult GetAuthors(
            [FromQuery] AuthorsResourceParameters authorsResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {

            try
            {

                if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
                {
                    return ValidationProblem($"ps-344-webAPI-20220306-2327: Invalid media type [{mediaType}]");
                }

                var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParameters);

                #region [20220304-2120] 
                var previousPageLink = authorsFromRepo.HasPreviousPage ? createAuthorResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
                var nextPageLink = authorsFromRepo.HasNextPage ? createAuthorResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

                var pagingMetaData = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    totalPages = authorsFromRepo.TotalPages,
                    previousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagingMetaData));

                #endregion [20220304-2120] 


                if (parsedMediaType.MediaType == Constants.MEDIA_TYPE_APPLICATION_VND_MARGIN_HATEOAS_JSON)
                {

                    // 03/06/2022 09:06 pm - SSN - [20220306-2054] - [003] - M05-06 - Demo - Implementing HATEOAS support for collection resource
                    var links = createLinksForAuthors(authorsResourceParameters, authorsFromRepo.HasPreviousPage, authorsFromRepo.HasNextPage);
                    var shapedAuthors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData_v2(authorsResourceParameters.Fields);

                    var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
                          {
                              var authorAsDic = author as IDictionary<string, object>;
                              var authorLinks = CreateLinksForAuthor((Guid)authorAsDic["Id"], null);
                              authorAsDic.Add("links", authorLinks);
                              return authorAsDic;
                          });

                    // 03/06/2022 05:33 pm - SSN - [20220305-1512] - [006] - M04-05 - Demo - Data shaping single resources
                    // return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData(authorsResourceParameters.Fields));

                    // return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData_v2(authorsResourceParameters.Fields));
                    var linkedCollectionResource = new
                    {
                        value = shapedAuthorsWithLinks,
                        links
                    };

                    // return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData_v2(authorsResourceParameters.Fields));
                    return Ok(linkedCollectionResource);
                }
                else
                {
                    return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData_v2(authorsResourceParameters.Fields));
                }

            }
            catch (Exception ex)
            {
                // Todo: Need log.
                return ValidationProblem(ex.Message);
            }

        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        // 03/06/2022 04:46 pm - SSN - [20220305-1512] - [002] - M04-05 - Demo - Data shaping single resources
        // Add fields
        // 03/06/2022 10:08 pm - SSN - [20220306-2206] - [001] - M06-04 - Demo - HATEOAS and content negotiation
        // Get Accept header
        public IActionResult GetAuthor(Guid authorId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            // 03/06/2022 06:41 pm - SSN - [20220306-1816] - [001] - M04-06 - Demo - Taking consumer errors into account when shaping data
            // Add try/catch block
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
                {
                    return ValidationProblem($"ps-344-webAPI-20220306-2213: Invalid media type [{mediaType}]");
                }

                var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

                if (authorFromRepo == null)
                {
                    return NotFound();
                }

                // 03/06/2022 10:18 pm - SSN - [20220306-2206] - [003] - M06-04 - Demo - HATEOAS and content negotiation
                if (parsedMediaType.MediaType == Constants.MEDIA_TYPE_APPLICATION_VND_MARGIN_HATEOAS_JSON)
                {


                    // 03/06/2022 08:07 pm - SSN - [20220306-1937] - [003] - M05-04 - Demo - Implementing HATEOAS support for a single resource
                    var links = CreateLinksForAuthor(authorId, fields);

                    // 03/06/2022 04:47 pm - SSN - [20220305-1512] - [003] - M04-05 - Demo - Data shaping single resources
                    // return Ok(_mapper.Map<AuthorDto>(authorFromRepo));

                    //return Ok(_mapper.Map<AuthorDto>(authorFromRepo).ShapeData_v2(fields));
                    var dic = _mapper.Map<AuthorDto>(authorFromRepo).ShapeData_v2(fields) as IDictionary<string, object>;
                    dic.Add("links", links);

                    return Ok(dic);
                }
                else
                {
                    return Ok(_mapper.Map<AuthorDto>(authorFromRepo).ShapeData_v2(fields));
                }
            }
            catch (Exception ex)
            {

                // Todo: Need log.
                return ValidationProblem(ex.Message);
            }
        }

        [HttpPost(Name = "CreateAuthor")]
        // 03/06/2022 11:16 pm - SSN - [20220306-2206] - [006] - M06-04 - Demo - HATEOAS and content negotiation
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return ValidationProblem($"ps-344-webAPI-20220306-2317: Invalid media type [{mediaType}]");
            }

            var authorEntity = _mapper.Map<Entities.Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            if (parsedMediaType.MediaType == Constants.MEDIA_TYPE_APPLICATION_VND_MARGIN_HATEOAS_JSON)
            {
                // 03/06/2022 08:23 pm - SSN - [20220306-2018] - [001] - M05-05 - Demo - Implementng HATEOAS support after POSTing
                var links = CreateLinksForAuthor(authorToReturn.Id, null);
                var dic = authorToReturn.ShapeData_v2(null) as IDictionary<string, object>;
                dic.Add("links", links);

                //return CreatedAtRoute("GetAuthor",
                //    new { authorId = authorToReturn.Id },
                //    authorToReturn);
                return CreatedAtRoute("GetAuthor",
                       new { authorId = dic["Id"] },
                       dic);
            }

            return CreatedAtRoute("GetAuthor",
                new { authorId = authorToReturn.Id },
                authorToReturn);

        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{authorId}", Name = "DeleteAuthor")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteAuthor(authorFromRepo);

            _courseLibraryRepository.Save();

            return NoContent();
        }


        // 03/04/2022 09:25 pm - SSN - [20220304-2120] - [002] - M02-10 - Demo - Returning pagination metadata
        private string createAuthorResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType resourceUriType)
        {
            switch (resourceUriType)
            {
                case ResourceUriType.PreviousPage:
                    return CreateLink(authorsResourceParameters, -1);

                case ResourceUriType.NextPage:
                    return CreateLink(authorsResourceParameters, 1);

                case ResourceUriType.Current:
                default:
                    return CreateLink(authorsResourceParameters, 0);

            }
        }

        private string CreateLink(AuthorsResourceParameters authorsResourceParameters, int skipValue)
        {
            return Url.Link("GetAuthors",
                                new
                                {
                                    fields = authorsResourceParameters.Fields,
                                    pageNumber = authorsResourceParameters.PageNumber + skipValue,
                                    pageSize = authorsResourceParameters.PageSize,
                                    mainCategory = authorsResourceParameters.MainCategory,
                                    searchQuery = authorsResourceParameters.SearchQuery,
                                    orderBy = authorsResourceParameters.OrderBy
                                });
        }


        // 03/06/2022 07:44 pm - SSN - [20220306-1937] - [002] - M05-04 - Demo - Implementing HATEOAS support for a single resource
        private IEnumerable<LinkDTO> CreateLinksForAuthor(Guid authorId, string fields)
        {
            var links = new List<LinkDTO>();

            var payLoad = new ExpandoObject();

            ((IDictionary<string, object>)payLoad).Add("authorId", authorId);

            if (!string.IsNullOrWhiteSpace(fields)) ((IDictionary<string, object>)payLoad).Add("fields", fields);

            links.Add(new LinkDTO(Url.Link("GetAuthor", payLoad), "self", "GET"));
            links.Add(new LinkDTO(Url.Link("DeleteAuthor", new { authorId }), "delete_author", "GET"));
            links.Add(new LinkDTO(Url.Link("CreateCourseForAuthor", new { authorId }), "create_course_for_author", "POST"));
            links.Add(new LinkDTO(Url.Link("GetCoursesForAuthor", new { authorId }), "get_courses_for_author", "GET"));

            return links;
        }

        // 03/06/2022 08:59 pm - SSN - [20220306-2054] - [001] - M05-06 - Demo - Implementing HATEOAS support for collection resource
        private IEnumerable<LinkDTO> createLinksForAuthors(AuthorsResourceParameters authorsResourceParameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDTO>();

            links.Add(new LinkDTO(createAuthorResourceUri(authorsResourceParameters, ResourceUriType.Current), "self", "GET"));

            if (hasPrevious)
            {

                links.Add(new LinkDTO(createAuthorResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage), "previous-page", "GET"));
            }

            if (hasNext)
            {

                links.Add(new LinkDTO(createAuthorResourceUri(authorsResourceParameters, ResourceUriType.NextPage), "next-page", "GET"));
            }


            return links;

        }
    }
}
