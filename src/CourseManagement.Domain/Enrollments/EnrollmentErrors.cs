using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Enrollments
{
    public class EnrollmentErrors
    {
        public static readonly Error IdCannotBeEqualToNorBelowZero = Error.Validation(
            "Enrollment.IdCannotBeEqualToNorBelowZero",
            "Id can not be equal to, nor lesser than, zero.");

        public static readonly Error EnrollmentAlreadyHasAttendance = Error.Validation(
            "Enrollment.EnrollmentAlreadyHasAttendance",
            "Attendance already been signed in this enrollment.");
    }
}

/*
        public static readonly Error OptionalCompletionCannotBeBelowZero = Error.Validation(
            "Enrollment.OptionalCompletionCannotBeBelowZero",
            "Optional completion cannot be below zero.");
*/
