using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record MaterialDto : IHasAffectedIds
    {
        public required int MaterialId { get; set; }
        public required string Path { get; set; }
        public required bool IsVideo { get; set; }

        public int AffectedId { get; set; }
    }
}
