using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record MaterialDto : IHasAffectedIds
    {
        public required int MaterialId { get; set; }
        public required int SessionId { get; set; }
        public required string SessionName { get; set; }
        public required int CourseId { get; set; }
        public required string CourseSubject { get; set; }
        public required string Path { get; set; }
        public required bool IsVideo { get; set; }

        public int AffectedId { get; set; }
    }

    public record DownloadMaterialFileInfo : IHasAffectedIds
    {
        public required string Path { get; set; }
        public required string FileName { get; set; }

        public int AffectedId { get; set; }
    }
}
