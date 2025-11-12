using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Contracts.Skills;

namespace CourseManagement.Contracts.Courses
{
    public record CreateCourseRequest
    {
        public required string Subject { get; set; }
        public required string Description { get; set; }
        public required AddCourseEligibilityRequest Eligibility { get; set; }
        public required List<AddSkillToCourseRequest> Skills { get; set; }
    }
}
