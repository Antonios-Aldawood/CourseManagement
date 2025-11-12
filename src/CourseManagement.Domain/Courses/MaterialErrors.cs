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
    }
}
