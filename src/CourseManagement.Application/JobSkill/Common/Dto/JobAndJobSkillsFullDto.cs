using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.JobsSkills;

namespace CourseManagement.Application.JobSkill.Common.Dto
{
    public record JobAndJobSkillsFullDto : IHasAffectedIds
    {
        public int? JobId { get; set; } = null;
        public string? JobTitle { get; set; } = null;
        public List<JobsSkills> JobSkills { get; set; } = [];

        public int AffectedId { get; set; }
    }
}
