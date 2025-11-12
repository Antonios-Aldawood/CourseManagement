using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record AddCourseEligibilityRequest
    {
        public required string Subject { get; set; }
        public string? Position { get; set; }
        public List<int>? PositionIds { get; set; }
        public string? Department { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public string? Job { get; set; }
        public List<int>? JobIds { get; set; }
    }
}
