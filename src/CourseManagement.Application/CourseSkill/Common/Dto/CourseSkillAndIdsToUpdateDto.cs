using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.CoursesSkills;

namespace CourseManagement.Application.CourseSkill.Common.Dto
{
    public record CourseSkillAndIdsToUpdateDto : IHasAffectedIds
    {
        public CoursesSkills? CourseSkill { get; set; }
        public int? CourseId { get; set; }
        public int? SkillId { get; set; }

        public int AffectedId { get; set; }
    }
}
