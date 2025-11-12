using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;

namespace CourseManagement.Application.Enrollments.Queries.GetUserCourses
{
    public class GetUserCoursesQueryHandler(
        IEnrollmentsRepository enrollmentsRepository,
        IUsersRepository usersRepository
        ) : IRequestHandler<GetUserCoursesQuery, ErrorOr<List<EnrollmentDto>>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ErrorOr<List<EnrollmentDto>>> Handle(GetUserCoursesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _usersRepository.GetByIdAsync(query.userId);
                if (user is null)
                {
                    return Error.Validation(description: "User not found.");
                }

                var courses = await _enrollmentsRepository.GetEnrollmentsForUser(user.Id);

                return courses;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
