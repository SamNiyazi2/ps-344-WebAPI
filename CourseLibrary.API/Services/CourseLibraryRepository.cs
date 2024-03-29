﻿using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext _context;
        private readonly IPropertyMappingService propertyMappingService;

        // 03/05/2022 09:45 am - SSN - [20220305-0715] - [008] - M03-05 - Creating a property mapping service
        // Add IPropertyMappingService
        public CourseLibraryRepository(CourseLibraryContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)) ?? throw new ArgumentNullException($"ps-344-webAPI-20220305-0947: Null [{nameof(context)}]"); ;
            this.propertyMappingService = propertyMappingService ?? throw new ArgumentNullException($"ps-344-webAPI-20220305-0948: Null [{nameof(propertyMappingService)}]");
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToList();
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList<Author>();
        }

        // 03/04/2022 07:50 pm - SSN - [20220304-1924] - [003] - M02-09 - Demo - Improving reuse with a PagedList<T> class

        //        public IEnumerable<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if (authorsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            // 03/04/2022 05:12 pm - SSN - [20220304-1649] - [003] - M02-07 - Demo = Paging through collection resources
            //if (string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory)
            //     && string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            //{
            //    return GetAuthors();
            //}

            var collection = _context.Authors as IQueryable<Author>;

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {

                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                    || a.FirstName.Contains(searchQuery)
                    || a.LastName.Contains(searchQuery));
            }


            // 03/05/2022 06:53 am - SSN - [20220305-0647] - [002] - M03-03 - Demo - Sorting resource collections
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.OrderBy))
            {
                //if (authorsResourceParameters.OrderBy.Trim().ToLower() == "name")
                //{
                //    collection = collection.OrderBy(a => a.LastName).ThenBy(a => a.FirstName);
                //}

                collection = collection.ApplySort(authorsResourceParameters.OrderBy, propertyMappingService.GetPropertyMapping<AuthorDto, Author>());
            }

            // 03/04/2022 05:08 pm - SSN - [20220304-1649] - [002] - M02-07 - Demo = Paging through collection resources

            int pageNumber = authorsResourceParameters.PageNumber;
            int pageSize = authorsResourceParameters.PageSize;

            // 03/04/2022 07:51 pm - SSN - [20220304-1924] - [004] - M02-09 - Demo - Improving reuse with a PagedList<T> class
            // collection = collection.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            // return collection.ToList();

            return PagedList<Author>.Create(collection, pageNumber, pageSize);

        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
