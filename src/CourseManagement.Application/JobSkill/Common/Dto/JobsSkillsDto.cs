using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.JobSkill.Common.Dto
{
    public record JobsSkillsDto : IHasAffectedIds
    {
        public required int JobSkillId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public required int JobId { get; set; }
        public string? JobTitle { get; set; }
        public required int SkillId { get; set; }
        public string? Skill { get; set; }
        public required int Weight { get; set; }

        public int AffectedId { get; set; }
    }
}
