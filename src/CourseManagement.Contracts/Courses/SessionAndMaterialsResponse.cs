using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record SessionAndMaterialsResponse(
        int SessionId,
        string Name,
        int CourseId,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate,
        bool IsOffline,
        List<MaterialResponse>? Materials);
}
