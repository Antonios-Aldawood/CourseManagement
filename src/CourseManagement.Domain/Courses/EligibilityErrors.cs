using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class EligibilityErrors
    {
        public static readonly Error PositionMismatch = Error.Validation(
            "Eligibility.PositionMismatch",
            "Eligibility for position that isn't one of ours.");

        public static readonly Error KeyUnknown = Error.Validation(
            "Eligibility.KeyUnknown",
            "Key not 'Job', nor 'Position', nor 'Department'.");

        public static readonly Error EligibilityValueDoesNotBelongToPositionKey = Error.Validation(
            "Eligibility.EligibilityValueDoesNotBelongToPositionKey",
            "Can't assign a value to position key, outside of the known positions.");

        public static readonly Error ValueBelowOrEqualToZero = Error.Validation(
            "Eligibility.ValueBelowOrEqualToZero",
            "Value is Id, so it cannot be below nor equal to zero.");
    }
}
