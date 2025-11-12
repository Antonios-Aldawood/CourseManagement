using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Domain.Services;

namespace CourseManagement.Application.Courses.Queries.GetCoursesForEligibilities
{
    public class GetCoursesForEligibilitiesAdminQueryHandler(
        ICoursesRepository coursesRepository,
        IUsersRepository usersRepository
        ) : IRequestHandler<GetCoursesForEligibilitiesAdminQuery, ErrorOr<List<CourseDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<CourseDto>>> Handle(GetCoursesForEligibilitiesAdminQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _usersRepository.GetUserWithJobAndDepartmentByIdAsync(query.userId);
                if (user == null)
                {
                    return Error.Validation(description: "User not found.");
                }

                int positionId = UserPositionCourseEligibilityService.ValidEligibilityPosition(user.Position);

                var courses = await _coursesRepository.GetAllCoursesThatMatchEligibilitiesAsync(positionId, user.DepartmentId, user.JobId);
                if (courses is null ||
                    courses.Count == 0)
                {
                    return Error.Validation(description: "No course found or no course matches user's eligibilities.");
                }

                return courses;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
