using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Skills
{
    public record AddSkillToCourseRequest
    {
        public required string SkillName { get; set; }
        public required int LevelCap { get; set; }
        public required int Weight { get; set; }
    }
}
