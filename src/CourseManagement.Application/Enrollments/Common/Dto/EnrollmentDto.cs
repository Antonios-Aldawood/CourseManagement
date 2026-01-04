using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Enrollments.Common.Dto
{
    public record EnrollmentDto : IHasAffectedIds
    {
        public required int EnrollmentId { get; set; }
        public required int UserId { get; set; }
        public required string UserAlias { get; set; }
        public required int CourseId { get; set; }
        public required string CourseSubject { get; set; }
        public required bool IsOptional { get; set; }
        public required bool IsCompleted { get; set; }
        public required bool IsConfirmed { get; set; }

        public int AffectedId { get; set; }
    }
}
