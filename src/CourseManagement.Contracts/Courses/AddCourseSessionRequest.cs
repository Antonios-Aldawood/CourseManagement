using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record AddCourseSessionRequest
    {
        public required string Subject { get; set; }
        public required string Name { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
        public required int TrainerId { get; set; }
        public required bool IsOffline { get; set; }
        public int? Seats { get; set; }
        public string? Link { get; set; }
        public string? App { get; set; }
    }
}
