using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Domain.Courses;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Common.Dto
{
    public record SessionAndMaterialsDto : IHasAffectedIds
    {
        public required int SessionId { get; set; }
        public required string Name { get; set; }
        public required int CourseId { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
        public required int TrainerId { get; set; }
        public required bool IsOffline { get; set; }
        public int? Seats { get; set; }
        public string? Link { get; set; }
        public string? App { get; set; }
        public required List<MaterialDto> Materials { get; set; }

        public int AffectedId { get; set; }
    }
}
