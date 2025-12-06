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

namespace CourseManagement.Application.Courses.Queries.GetCourse
{
    public class GetCourseQueryHandler(
        ICoursesRepository coursesRepository,
        IMessageProducer messageProducer
        ) : IRequestHandler<GetCourseQuery, ErrorOr<List<CourseDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IMessageProducer _messageProducer = messageProducer;

        public async Task<ErrorOr<List<CourseDto>>> Handle(GetCourseQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundCourses = await _coursesRepository.GetCourseBySubjectAsync(query.subject);

                if (foundCourses is not List<Course> courses ||
                    foundCourses.Count == 0)
                {
                    return Error.Validation(description: "Courses not found.");
                }

                List<CourseDto> courseResponse = [];

                foreach (Course course in courses)
                {
                    CourseDto dto = CourseDto.AddCourseDto(course);

                    courseResponse.Add(dto);
                }

                await _messageProducer.SendMessage(foundCourses.First().Eligibilities);

                return courseResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
