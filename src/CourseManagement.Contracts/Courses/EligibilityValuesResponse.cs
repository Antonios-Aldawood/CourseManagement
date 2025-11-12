using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record EligibilityValuesResponse(
        int EligibilityId,
        string Course,
        string Key,
        string Value);
}
