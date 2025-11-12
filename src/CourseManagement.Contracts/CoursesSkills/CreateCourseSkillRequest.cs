using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.CoursesSkills
{
    public record CreateCourseSkillRequest
    {
        public required int CourseId { get; set; }
        public required int SkillId { get; set; }
        public required int Weight { get; set; }
    }
}
