using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {

            try
            {

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

                // 03/06/2022 05:33 pm - SSN - [20220305-1512] - [006] - M04-05 - Demo - Data shaping single resources
                // return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData(authorsResourceParameters.Fields));
                return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData_v2(authorsResourceParameters.Fields));

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
        public IActionResult GetAuthor(Guid authorId, string fields)
        {
            // 03/06/2022 06:41 pm - SSN - [20220306-1816] - [001] - M04-06 - Demo - Taking consumer errors into account when shaping data
            // Add try/catch block
            try
            {
                var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

                if (authorFromRepo == null)
                {
                    return NotFound();
                }

                // 03/06/2022 04:47 pm - SSN - [20220305-1512] - [003] - M04-05 - Demo - Data shaping single resources
                // return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
                return Ok(_mapper.Map<AuthorDto>(authorFromRepo).ShapeData_v2(fields));

            }
            catch (Exception ex)
            {

                // Todo: Need log.
                return ValidationProblem(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = _mapper.Map<Entities.Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
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

        [HttpDelete("{authorId}")]
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

    }
}
