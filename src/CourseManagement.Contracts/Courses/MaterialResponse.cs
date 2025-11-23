using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record MaterialResponse(
        int MaterialId,
        int SessionId,
        string SessionName,
        int CourseId,
        string CourseSubject,
        string Path,
        bool IsVideo);
}
