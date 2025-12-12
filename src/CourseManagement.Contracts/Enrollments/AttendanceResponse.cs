using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Enrollments
{
    public record AttendanceResponse(
        int AttendanceId,
        int EnrollmentId,
        int UserId,
        string UserAlias,
        int SessionId,
        string SessionName,
        int SessionTrainerId,
        string SessionTrainerAlias,
        int CourseId,
        string CourseSubject,
        DateTimeOffset DateAttended);
}
