using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Enrollments
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public bool IsOptional { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
        public List<Attendance>? Attendances { get; set; }

        private Enrollment(
            int userId,
            int courseId,
            bool isOptional)
        {
            UserId = userId;
            CourseId = courseId;
            IsOptional = isOptional;
        }

        private ErrorOr<Success> CheckIfUserCourseEnrollmentIsValid()
        {
            if (UserId <= 0 ||
                CourseId <= 0)
            {
                return EnrollmentErrors.IdCannotBeEqualToNorBelowZero;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfUserCourseEnrollmentIdsAreValid(
            int userId,
            int courseId)
        {
            if (userId <= 0 ||
                courseId <= 0)
            {
                return EnrollmentErrors.IdCannotBeEqualToNorBelowZero;
            }
            
            return Result.Success;
        }

        public static ErrorOr<Enrollment> CreateUserCourseEnrollment(
            int userId,
            int courseId,
            bool isOptional)
        {
            var userCourse = new Enrollment(
                userId: userId,
                courseId: courseId,
                isOptional: isOptional);

            if (userCourse.CheckIfUserCourseEnrollmentIsValid() != Result.Success)
            {
                return userCourse.CheckIfUserCourseEnrollmentIsValid().Errors;
            }

            return userCourse;
        }

        public ErrorOr<Enrollment> UpdateUserCourseEnrollment(
            int userId,
            int courseId,
            bool isOptional)
        {
            var userCourseValidity = CheckIfUserCourseEnrollmentIdsAreValid(
                userId: userId,
                courseId: courseId);
            
            if (userCourseValidity != Result.Success)
            {
                return userCourseValidity.Errors;
            }

            UserId = userId;
            CourseId = courseId;
            IsOptional = isOptional;

            return this;
        }

        public ErrorOr<Attendance> AddEnrollmentAttendance(
            int sessionId,
            DateTimeOffset dateAttended)
        {
            if (Attendances != null &&
                Attendances.Count != 0 &&
                Attendances.Any(a => a.EnrollmentId == Id && a.SessionId == sessionId))
            {
                return EnrollmentErrors.EnrollmentAlreadyHasAttendance;
            }

            var attendance = Attendance.CreateAttendance(
                enrollmentId: Id,
                sessionId: sessionId,
                dateAttended: dateAttended);

            if (attendance.IsError)
            {
                return attendance.Errors;
            }

            Attendances?.Add(attendance.Value);

            return attendance.Value;
        }
    }
}

/*
//Can be substituted with database queries, but I thought it's faster to keep track of it here.
public int OptionalCompletion { get; set; }

//In CheckIfUserCourseEnrollmentIsValid after return EnrollmentErrors.IdCannotBeEqualToNorBelowZero; condition:
            if (OptionalCompletion < 0)
            {
                return EnrollmentErrors.OptionalCompletionCannotBeBelowZero;
            }

//In CreateUserCourseEnrollment after return userCourseValidity.Errors; condition:
            if (isOptional)
            {
                userCourse.OptionalCompletion++;
            }

//In UpdateUserCourseEnrollment after IsOptional = isOptional:
            if (isOptional)
            {
                OptionalCompletion++;
            }
*/