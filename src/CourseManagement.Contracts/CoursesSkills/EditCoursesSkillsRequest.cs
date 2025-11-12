using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.CoursesSkills
{
    public record EditCoursesSkillsRequest
    {
        public required int OldId { get; set; }
        public int? Id { get; set; }
        public int? Weight { get; set; }
    }
}
