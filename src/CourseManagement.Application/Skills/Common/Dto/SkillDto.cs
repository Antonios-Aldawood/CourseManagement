using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Skills.Common.Dto
{
    public record SkillDto : IHasAffectedIds
    {
        public required int SkillId { get; set; }
        public required string SkillName { get; set; }
        public required int LevelCap { get; set; }
        public string? Course { get; set; }
        public int? CourseSkillId { get; set; }
        public int? Weight { get; set; }

        public int AffectedId { get; set; }
    }
}
