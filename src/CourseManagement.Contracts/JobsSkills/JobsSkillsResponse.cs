using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.JobsSkills
{
    public record JobsSkillsResponse(
        int JobSkillId,
        int JobId,
        int SkillId,
        int Weight,
        int? DepartmentId = null,
        string? Department = null,
        string? Job = null,
        string? Skill = null);
}
