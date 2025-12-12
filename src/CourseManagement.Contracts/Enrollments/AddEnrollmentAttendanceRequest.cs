using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Enrollments
{
    public record AddEnrollmentAttendanceRequest
    {
        public required int TrainerId { get; set; }
        public required int EnrollmentId { get; set; }
        public required int CourseId { get; set; }
        public required int SessionId { get; set; }
        public required DateTimeOffset DateAttended { get; set; }
    }
}
