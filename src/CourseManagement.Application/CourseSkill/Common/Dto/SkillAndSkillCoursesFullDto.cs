using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.CoursesSkills;

namespace CourseManagement.Application.CourseSkill.Common.Dto
{
    public record SkillAndSkillCoursesFullDto : IHasAffectedIds
    {
        public int? SkillId { get; set; } = null;
        public string? SkillName { get; set; } = null;
        public List<CoursesSkills> CoursesSkill { get; set; } = [];

        public int AffectedId { get; set; }
    }
}
