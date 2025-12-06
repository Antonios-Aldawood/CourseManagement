using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CourseManagement.Contracts.Courses
{
    public record AddCourseSessionMaterialRequest
    {
        public required int CourseId { get; set; }
        public required int SessionId { get; set; }
        //public required string Path { get; set; }
        public required IFormFile File { get; set; }
        public required bool IsVideo { get; set; }
    }
}
