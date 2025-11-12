using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.CourseSkill.Common.Dto
{
    public record CoursesSkillsIdsDto : IHasAffectedIds
    {
        public required int CourseSkillId { get; set; }
        public required int CourseId { get; set; }
        public required int SkillId { get; set; }
        public required int Weight { get; set; }

        public int AffectedId { get; set; }
    }
}
