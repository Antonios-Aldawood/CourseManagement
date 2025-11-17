using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record MaterialResponse(
        int MaterialId,
        string Path,
        bool IsVideo);
}
