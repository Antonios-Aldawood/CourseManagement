using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class MaterialErrors
    {
        public static readonly Error SessionIdIsAbnormal = Error.Validation(
            "Material.SessionIdIsAbnormal",
            "Session Id has to belong to a persisted session.");

        public static readonly Error PathIsTooLong = Error.Validation(
            "Material.PathIsTooLong",
            "Material path must be under 500 characters, please place your file in another path.");
    }
}
