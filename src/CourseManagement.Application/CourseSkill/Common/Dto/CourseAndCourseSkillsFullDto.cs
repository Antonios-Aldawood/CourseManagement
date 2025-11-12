using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.CoursesSkills;

namespace CourseManagement.Application.CourseSkill.Common.Dto
{
    public record CourseAndCourseSkillsFullDto : IHasAffectedIds
    {
        public int? CourseId { get; set; } = null;
        public string? CourseSubject { get; set; } = null;
        public List<CoursesSkills> CourseSkills { get; set; } = [];

        public int AffectedId { get; set; }
    }
}
