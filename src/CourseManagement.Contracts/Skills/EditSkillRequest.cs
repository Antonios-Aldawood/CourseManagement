using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Skills
{
    public record EditSkillRequest
    {
        public required string OldSkillSkillName { get; set; }
        public string? SkillName { get; set; }
        public int? LevelCap { get; set; }
    }
}
