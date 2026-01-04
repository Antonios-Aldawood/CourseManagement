using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Enrollments
{
    public class EnrollmentWaitingConfirmationErrors
    {
        public static readonly Error IdCannotBeEqualToNorBelowZero = Error.Validation(
            "EnrollmentWaitingConfirmation.IdCannotBeEqualToNorBelowZero",
            "Id can not be equal to, nor lesser than, zero.");
    }
}
