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

        private Attendance(
            int enrollmentId,
            int sessionId)
        {
            EnrollmentId = enrollmentId;
            SessionId = sessionId;
        }

        private ErrorOr<Success> CheckIfAttendanceIsValid()
        {
            if (EnrollmentId <= 0 ||
                SessionId <= 0)
            {
                return AttendanceErrors.IdCannotBeEqualToNorBelowZero;
            }

            return Result.Success;
        }

        internal ErrorOr<Attendance> CreateAttendance(
            int enrollmentId,
            int sessionId)
        {
            var attendance = new Attendance(
                enrollmentId: enrollmentId,
                sessionId: sessionId);

            if (attendance.CheckIfAttendanceIsValid() != Result.Success)
            {
                return attendance.CheckIfAttendanceIsValid().Errors;
            }

            return attendance;
        }
    }
}
