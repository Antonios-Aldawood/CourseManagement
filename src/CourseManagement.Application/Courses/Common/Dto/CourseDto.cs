using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Courses;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record CourseDto : IHasAffectedIds
    {
        public required int CourseId { get; set; }
        public required string Subject { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;

        public int AffectedId { get; set; }

        public static CourseDto AddCourseDto(Course course)
        {
            CourseDto dto = new CourseDto
            {
                CourseId = course.Id,
                Subject = course.Subject,
                Description = course.Description,
            };

            dto.AffectedId = course.Id;

            return dto;
        }

        public static CourseDto AddCourseDtoByAttributes(
            int courseId,
            string subject,
            string description)
        {
            CourseDto dto = new CourseDto
            {
                CourseId = courseId,
                Subject = subject,
                Description = description,
            };

            dto.AffectedId = courseId;

            return dto;
        }
    }
}
