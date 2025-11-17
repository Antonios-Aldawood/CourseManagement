using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Courses
{
    public record UpdateCourseSessionMaterialPlacementRequest
    {
        public required int OldMaterialId { get; set; }
        public required int OldSessionId { get; set; }
        public required int OldCourseId { get; set; }
        public required string NewMaterialSessionName { get; set; }
        public required int? NewCourseId { get; set; }
    }
}
