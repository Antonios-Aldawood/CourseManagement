using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Courses;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.CourseSkill.Common.Dto
{
    public record CoursesSkillsWithCourseAndSkillDto : IHasAffectedIds
    {
        public required int CourseSkillId { get; set; }
        public required Course Course { get; set; }
        public required Skill Skill { get; set; }
        public required int Weight { get; set; }

        public int AffectedId { get; set; }
    }
}
