using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.JobsSkills
{
    public record EditJobSkillRequest
    {
        public required int OldJobSkillId { get; set; }
        public required int JobId { get; set; }
        public required int SkillId { get; set; }
        public int? Weight { get; set; }
    }
}
