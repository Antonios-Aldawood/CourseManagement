using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Skills
{
    public record SkillResponse(
        int SkillId,
        string SkillName,
        int LevelCap,
        string? Course = null,
        int? CourseSkillId = null,
        int? Weight = null);
}
