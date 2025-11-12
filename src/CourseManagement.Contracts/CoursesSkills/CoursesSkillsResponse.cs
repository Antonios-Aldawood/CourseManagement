using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.CoursesSkills
{
    public record CoursesSkillsResponse(
        int CourseSkillId,
        int CourseId,
        int SkillId,
        int Weight,
        string? Course = null,
        string? Skill = null);
}
