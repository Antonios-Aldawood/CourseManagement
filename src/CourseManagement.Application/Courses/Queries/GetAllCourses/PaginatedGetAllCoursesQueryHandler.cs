using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Dto;
using CourseManagement.Application.Courses.Common.Dto;

namespace CourseManagement.Application.Courses.Queries.GetAllCourses
{
    public class PaginatedGetAllCoursesQueryHandler(
        ICoursesRepository coursesRepository
        ) : IRequestHandler<PaginatedGetAllCoursesQuery, ErrorOr<PagedResult<CourseDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;

        public async Task<ErrorOr<PagedResult<CourseDto>>> Handle(PaginatedGetAllCoursesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var (courses, totalCount) = await _coursesRepository
                    .GetAllPaginatedCoursesAsync(query.pageNumber, query.pageSize);

                var courseResponse = courses
                    .Select(CourseDto.AddCourseDto)
                    .ToList();

                return new PagedResult<CourseDto>
                {
                    Items = courseResponse,
                    PageNumber = query.pageNumber,
                    PageSize = query.pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
