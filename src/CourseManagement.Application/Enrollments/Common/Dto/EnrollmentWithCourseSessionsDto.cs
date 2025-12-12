using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Enrollments;

namespace CourseManagement.Application.Enrollments.Common.Dto
{
    public record EnrollmentWithCourseSessionsDto : IHasAffectedIds
    {
        public required Enrollment Enrollment { get; set; }
        public required int CourseId { get; set; }
        public required string CourseSubject { get; set; }
        public required int SessionId { get; set; }
        public required string SessionName { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
        public required int SessionTrainerId { get; set; }
        public required string SessionTrainerAlias { get; set; }
        public required int UserId { get; set; }
        public required string UserAlias { get; set; }

        public int AffectedId { get; set; }
    }
}
