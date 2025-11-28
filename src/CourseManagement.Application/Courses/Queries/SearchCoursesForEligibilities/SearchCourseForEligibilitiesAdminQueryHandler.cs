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

namespace CourseManagement.Application.Courses.Queries.SearchCoursesForEligibilities
{
    public class SearchCourseForEligibilitiesAdminQueryHandler(
        ICoursesRepository coursesRepository,
        IUsersRepository usersRepository
        ) : IRequestHandler<SearchCourseForEligibilitiesAdminQuery, ErrorOr<List<CourseDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<CourseDto>>> Handle(SearchCourseForEligibilitiesAdminQuery query, CancellationToken cancellationToken)
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

                var searchedCourses = courses
                    .Where(c => c.Subject.ToLower().Contains(query.subject.ToLower()))
                    .ToList();

                return searchedCourses;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
