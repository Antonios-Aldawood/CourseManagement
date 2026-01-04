using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Application.Enrollments.Common.Dto
{
    public record EnrollmentWithUserAndCourseDto : IHasAffectedIds
    {
        public required Enrollment Enrollment { get; set; }
        public required string UserAlias { get; set; }
        public required string CourseSubject { get; set; }

        public int AffectedId { get; set; }
    }
}
