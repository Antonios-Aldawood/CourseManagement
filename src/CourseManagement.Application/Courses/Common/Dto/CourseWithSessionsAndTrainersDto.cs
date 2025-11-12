using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record CourseWithSessionsAndTrainersDto : IHasAffectedIds
    {
        public required string CourseSubject { get; set; }
        public required Session CourseSession { get; set; }
        public required string TrainerName { get; set; }

        public int AffectedId { get; set; }
    }
}
