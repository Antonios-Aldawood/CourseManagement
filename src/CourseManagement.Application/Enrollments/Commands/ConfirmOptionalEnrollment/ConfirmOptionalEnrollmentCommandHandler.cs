using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Enrollments.Common.Dto;

namespace CourseManagement.Application.Enrollments.Commands.ConfirmOptionalEnrollment
{
    public class ConfirmOptionalEnrollmentCommandHandler(
        IEnrollmentsRepository enrollmentsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<ConfirmOptionalEnrollmentCommand, ErrorOr<EnrollmentDto>>
    {
        private readonly IEnrollmentsRepository _enrollmentsRepository = enrollmentsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<EnrollmentDto>> Handle(ConfirmOptionalEnrollmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var enrollment = await _enrollmentsRepository.GetEnrollmentWithUserAndCourseNamesByIdAsync(command.enrollmentId);
                if (enrollment == null)
                {
                    return Error.Validation(description: "Enrollment not found.");
                }

                if (enrollment.Enrollment.IsOptional == false)
                {
                    return Error.Validation(description: "Enrollment was added by admin, therefore it is already confirmed.");
                }

                if (enrollment.Enrollment.IsConfirmed)
                {
                    return Error.Validation(description: "Enrollment already confirmed.");
                }

                enrollment.Enrollment.ConfirmEnrollment();

                await _unitOfWork.CommitChangesAsync();

                return new EnrollmentDto
                {
                    EnrollmentId = enrollment.Enrollment.Id,
                    UserId = enrollment.Enrollment.UserId,
                    UserAlias = enrollment.UserAlias,
                    CourseId = enrollment.Enrollment.CourseId,
                    CourseSubject = enrollment.CourseSubject,
                    IsOptional = enrollment.Enrollment.IsOptional,
                    IsCompleted = enrollment.Enrollment.IsCompleted,
                    IsConfirmed = enrollment.Enrollment.IsConfirmed,

                    AffectedId = enrollment.Enrollment.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
