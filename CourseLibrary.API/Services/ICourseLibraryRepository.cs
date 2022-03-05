using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;
using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Services
{
    public interface ICourseLibraryRepository
    {
        IEnumerable<Course> GetCourses(Guid authorId);
        Course GetCourse(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        IEnumerable<Author> GetAuthors();


        // 03/04/2022 07:49 pm - SSN - [20220304-1924] - [002] - M02-09 - Demo - Improving reuse with a PagedList<T> class

        // IEnumerable<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);
        PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);


        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
        bool Save();
    }
}
