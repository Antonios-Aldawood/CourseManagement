using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record EditCourseRequest
    {
        public required string OldSubject { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
    }
}
