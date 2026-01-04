using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Enrollments
{
    public class EnrollmentWaitingConfirmation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; private set; }
        public bool IsOptional { get; private set; } = true;
        public bool IsCompleted { get; private set; } = false;
        public bool IsConfirmed { get; private set; } = false;

        private EnrollmentWaitingConfirmation(
            int userId,
            int courseId)
        {
            UserId = userId;
            CourseId = courseId;
        }

        public void ConfirmEnrollment()
        {
            IsConfirmed = true;
        }

        private ErrorOr<Success> CheckIfUserCourseEnrollmentWaitingConfirmationIsValid()
        {
            if (UserId <= 0 ||
                CourseId <= 0)
            {
                return EnrollmentErrors.IdCannotBeEqualToNorBelowZero;
            }

            return Result.Success;
        }

        public static ErrorOr<EnrollmentWaitingConfirmation> CreateEnrollmentWaitingConfirmation(
            int userId,
            int courseId)
        {
            var enrollmentWaitingConfirmation = new EnrollmentWaitingConfirmation(
                userId,
                courseId);

            if (enrollmentWaitingConfirmation.CheckIfUserCourseEnrollmentWaitingConfirmationIsValid() != Result.Success)
            {
                return enrollmentWaitingConfirmation.CheckIfUserCourseEnrollmentWaitingConfirmationIsValid().Errors;
            }

            return enrollmentWaitingConfirmation;
        }
    }
}
