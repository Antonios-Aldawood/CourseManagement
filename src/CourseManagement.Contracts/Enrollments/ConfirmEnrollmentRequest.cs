using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Enrollments
{
    public record ConfirmEnrollmentRequest
    {
        public required int EnrollmentId { get; set; }
    }
}
