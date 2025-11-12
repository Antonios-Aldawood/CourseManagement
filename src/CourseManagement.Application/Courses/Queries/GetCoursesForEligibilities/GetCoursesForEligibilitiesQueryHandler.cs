using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Behaviors;
using CourseManagement.Domain.Services;

namespace CourseManagement.Application.Courses.Queries.GetCoursesForEligibilities
{
    public class GetCoursesForEligibilitiesQueryHandler(
        ICoursesRepository coursesRepository,
        IUsersRepository usersRepository
        ) : IRequestHandler<GetCoursesForEligibilitiesQuery, ErrorOr<List<CourseDto>>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<CourseDto>>> Handle(GetCoursesForEligibilitiesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _usersRepository.GetUserWithJobAndDepartmentByIdAsync(query.userId);
                if (user is null)
                {
                    return Error.Validation(description: "User not found or has no job or department.");
                }
                
                var authenticatedId = IdentityBehavior.CheckIfAuthenticationIdMatch(query.headers, query.userId);
                if (authenticatedId != Result.Success)
                {
                    return authenticatedId.Errors;
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

/*
///// INSTEAD OF THE CURRENT WAY OF IMPLEMENTING USING TWO HANDLERS, THOUGH THE CURRENT WAY SEEMED BETTER /////
                var authenticatedRole = IdentityBehavior.CheckIfAuthenticationRoleMatch(query.headers, "Admin");
                if (authenticatedRole == Error.Forbidden(description: "From identity behavior - Entered role doesn't match user's role."))
                {
                    var authenticatedId = IdentityBehavior.CheckIfAuthenticationIdMatch(query.headers, query.userId);
                    if (authenticatedId != Result.Success)
                    {
                        return authenticatedId.Errors;
                    }
                }
*/