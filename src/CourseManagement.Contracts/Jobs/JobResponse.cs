using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Jobs
{
    public record JobResponse(
        int JobId,
        string Title,
        string Description,
        double? MinSalary = null,
        double? MaxSalary = null,
        string? Department = null);
}
