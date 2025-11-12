using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Jobs
{
    public record EditJobRequest
    {
        public required string OldTitle { get; set; }
        public string? Title { get; set; }
        public double? MinSalary { get; set; }
        public double? MaxSalary { get; set; }
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
    }
}
