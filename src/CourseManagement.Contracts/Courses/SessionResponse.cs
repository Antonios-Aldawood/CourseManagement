using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record SessionResponse(
        int SessionId,
        string SessionName,
        int CourseId,
        string CourseSubject,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate,
        int TrainerId,
        string TrainerAlias,
        bool IsOffline,
        int? Seats,
        string? Link,
        string? App);
}
