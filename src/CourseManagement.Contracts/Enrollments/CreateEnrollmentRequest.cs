using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Contracts.Enrollments
{
    public record CreateEnrollmentRequest
    {
        public required int UserId { get; set; }
        public required int CourseId { get; set; }
    }
}
