using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;

namespace CourseManagement.Application.Enrollments.Queries.GetCourseUsers
{
    public class GetCourseUsersQueryHandler(
        IEnrollmentsRepository enrollmentsRepository,
        ICoursesRepository coursesRepository
        ) : IRequestHandler<GetCourseUsersQuery, ErrorOr<List<EnrollmentDto>>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        private readonly ICoursesRepository _coursesRepository = coursesRepository;

        public async Task<ErrorOr<List<EnrollmentDto>>> Handle(GetCourseUsersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _coursesRepository.GetCourseByIdAsync(query.courseId);
                if (course is null)
                {
                    return Error.Validation(description: "Course not found.");
                }

                var users = await _enrollmentsRepository.GetEnrollmentsForCourse(course.Id);

                return users;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
