using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record CourseSkillsDto : IHasAffectedIds
    {
        public required int CourseId { get; set; }
        public required string Subject { get; set; }
        public required string Description { get; set; }
        public required string SkillName { get; set; }
        public required int Weight { get; set; }

        public int AffectedId { get; set; }
    }
}
