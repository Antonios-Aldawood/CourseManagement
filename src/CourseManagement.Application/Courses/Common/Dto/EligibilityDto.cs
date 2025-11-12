using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record EligibilityDto : IHasAffectedIds
    {
        public required int EligibilityId { get; set; }
        public required string Course { get; set; }
        public required string Key { get; set; }
        public required int Value { get; set; }

        public int AffectedId { get; set; }
    }
}
