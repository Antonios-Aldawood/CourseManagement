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
using CourseManagement.Domain.Courses;

namespace CourseManagement.Application.Courses.Queries.DownloadCourseSessionMaterial
{
    public class DownloadCourseSessionMaterialQueryHandler(
        ICoursesRepository coursesRepository,
        IUsersRepository usersRepository,
        IEnrollmentsRepository enrollmentsRepository
        ) : IRequestHandler<DownloadCourseSessionMaterialQuery, ErrorOr<DownloadMaterialFileInfo>>
    {
        private readonly ICoursesRepository _coursesRepository = coursesRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;

        public async Task<ErrorOr<DownloadMaterialFileInfo>> Handle(DownloadCourseSessionMaterialQuery query, CancellationToken cancellationToken)
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

                var courses = await _coursesRepository.GetAllCoursesWithSessionsAndMaterialsThatMatchEligibilitiesAsync(positionId, user.DepartmentId, user.JobId);
                if (courses.FirstOrDefault(c => c.Id == query.courseId) is not Course course)
                {
                    return Error.Validation(description: "User not eligible to download course material, or course was not found.");
                }

                var isEnrolled = await _enrollmentsRepository.ExistsForUserCourseAsync(user.UserId, course.Id);
                if (isEnrolled == false)
                {
                    return Error.Validation(description: "Can't download material because user isn't enrolled.");
                }
                
                var session = course.CheckIfCourseHasSessionBySessionId(query.sessionId);
                if (session.IsError)
                {
                    return session.Errors;
                }

                var material = session.Value.Materials?.FirstOrDefault(m => m.Id == query.materialId);
                if (material == null)
                {
                    return Error.Validation(description: "Material not found.");
                }

                if (string.IsNullOrWhiteSpace(material.Path))
                {
                    return Error.Validation(description: "Material does not have a valid path.");
                }

                return new DownloadMaterialFileInfo
                {
                    Path = material.Path,

                    AffectedId = material.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }

}
