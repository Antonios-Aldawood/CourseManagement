using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.CoursesSkills
{
    public record EditCourseSkillRequest
    {
        public required int OldCourseSkillId { get; set; }
        public required int CourseId { get; set; }
        public required int SkillId { get; set; }
        public int? Weight { get; set; } = null;
    }
}
