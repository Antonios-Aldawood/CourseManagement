using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Jobs.Common.Dto
{
    public record JobDto : IHasAffectedIds
    {
        public required int JobId { get; set; }
        public required string Title { get; set; } = string.Empty;
        public double? MinSalary { get; set; }
        public double? MaxSalary { get; set; }
        public required string Description { get; set; } = string.Empty;
        public required string Department { get; set; } = string.Empty;

        public int AffectedId { get; set; }
    }
}
