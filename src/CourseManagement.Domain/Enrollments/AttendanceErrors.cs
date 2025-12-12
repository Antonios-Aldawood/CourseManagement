using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Enrollments
{
    public class AttendanceErrors
    {
        public static readonly Error IdCannotBeEqualToNorBelowZero = Error.Validation(
            "Attendance.IdCannotBeEqualToNorBelowZero",
            "Id can not be equal to, nor lesser than, zero.");

        public static readonly Error DateAttendedCanNotBeInTheFuture = Error.Validation(
            "Attendance.DateAttendedCanNotBeInTheFuture",
            "Date attended can not be in the future.");
    }
}
