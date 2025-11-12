using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Jobs
{
    public record GetDepartmentsJobsRequest
    {
        public required List<string> Names { get; set; }
    }
}
