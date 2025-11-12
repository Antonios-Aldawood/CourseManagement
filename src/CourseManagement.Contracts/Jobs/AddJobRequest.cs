using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Jobs
{
    public record AddJobRequest
    {
        public required string Title { get; set; } = string.Empty;
        public required double MinSalary { get; set; } = 0;
        public required double MaxSalary { get; set; } = 0;
        public required string Description { get; set; } = string.Empty;
        public required string DepartmentName { get; set; } = string.Empty;
    }
}
