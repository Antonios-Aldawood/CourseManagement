using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record CourseWithSessionsAndMaterialsDto : IHasAffectedIds
    {
        public required int CourseId { get; set; }
        public required string Subject { get; set; }
        public required string Description { get; set; }
        public List<Session>? Sessions { get; set; }

        public int AffectedId { get; set; }

        public static CourseWithSessionsAndMaterialsDto AddDto(Course course)
        {
            return new CourseWithSessionsAndMaterialsDto
            {
                CourseId = course.Id,
                Subject = course.Subject,
                Description = course.Description,
                Sessions = course.Sessions,

                AffectedId = course.Id
            };
        }
    }
}
