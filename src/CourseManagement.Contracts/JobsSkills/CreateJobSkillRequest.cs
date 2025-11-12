using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.JobsSkills
{
    public record CreateJobSkillRequest
    {
        public required string JobTitle { get; set; }
        public required string SkillSkillName { get; set; }
        public required int Weight { get; set; }
    }
}
