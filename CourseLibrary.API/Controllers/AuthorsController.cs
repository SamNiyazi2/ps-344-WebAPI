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
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            [FromQuery] AuthorsResourceParameters authorsResourceParameters)
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


            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
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
                                    pageNumber = authorsResourceParameters.PageNumber + skipValue,
                                    pageSize = authorsResourceParameters.PageSize,
                                    mainCategory = authorsResourceParameters.MainCategory,
                                    searchQuery = authorsResourceParameters.SearchQuery,
                                    orderBy = authorsResourceParameters.OrderBy
                                }));
        }

    }
}
