using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.JobsSkills;

namespace CourseManagement.Application.JobSkill.Common.Dto
{
    public record SkillAndSkillJobsFullDto : IHasAffectedIds
    {
        public int? SkillId { get; set; } = null;
        public string? SkillName { get; set; } = null;
        public List<JobsSkills> JobsSkill { get; set; } = [];

        public int AffectedId { get; set; }
    }
}
