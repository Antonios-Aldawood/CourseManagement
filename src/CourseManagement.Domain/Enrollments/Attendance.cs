using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Enrollments
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int SessionId { get; set; }
        public DateTimeOffset DateAttended { get; set; }

        private Attendance(
            int enrollmentId,
            int sessionId,
            DateTimeOffset dateAttended)
        {
            EnrollmentId = enrollmentId;
            SessionId = sessionId;
            DateAttended = dateAttended;
        }

        private ErrorOr<Success> CheckIfAttendanceIsValid()
        {
            if (EnrollmentId <= 0 ||
                SessionId <= 0)
            {
                return AttendanceErrors.IdCannotBeEqualToNorBelowZero;
            }

            if (DateAttended > DateTimeOffset.UtcNow)
            {
                return AttendanceErrors.DateAttendedCanNotBeInTheFuture;
            }

            return Result.Success;
        }

        internal static ErrorOr<Attendance> CreateAttendance(
            int enrollmentId,
            int sessionId,
            DateTimeOffset dateAttended)
        {
            var attendance = new Attendance(
                enrollmentId: enrollmentId,
                sessionId: sessionId,
                dateAttended: dateAttended);

            if (attendance.CheckIfAttendanceIsValid() != Result.Success)
            {
                return attendance.CheckIfAttendanceIsValid().Errors;
            }

            return attendance;
        }
    }
}
