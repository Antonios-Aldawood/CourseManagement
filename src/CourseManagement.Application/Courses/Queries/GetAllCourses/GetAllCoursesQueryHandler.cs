using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Application.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryHandler(
        ICoursesRepository coursesRepository,
        IRedisCacheService redisCacheService
        ) : IRequestHandler<GetAllCoursesQuery, ErrorOr<List<CourseDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;

        public async Task<ErrorOr<List<CourseDto>>> Handle(GetAllCoursesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var cachedCourses = await _redisCacheService.GetData<List<CourseDto>>("Courses");
                if (cachedCourses != null &&
                    cachedCourses.Count != 0)
                {
                    return cachedCourses;
                }

                List<Course> courses = await _coursesRepository.GetAllCoursesAsync();
                List<CourseDto> courseResponse = [];

                foreach (Course course in courses)
                {
                    CourseDto dto = CourseDto.AddCourseDto(course);

                    courseResponse.Add(dto);
                }

                await _redisCacheService.SetData("Courses", courseResponse);

                return courseResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
