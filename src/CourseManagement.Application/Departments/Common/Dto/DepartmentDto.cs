using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Departments;

namespace CourseManagement.Application.Departments.Common.Dto
{
    public record DepartmentDto : IHasAffectedIds
    {
        public required int DepartmentId { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required int MinMembers { get; set; }
        public required int MaxMembers { get; set; }
        public required string Description { get; set; } = string.Empty;

        public int AffectedId { get; set; }

        public static DepartmentDto AddDto(Department department)
        {
            DepartmentDto dto = new DepartmentDto
            {
                DepartmentId = department.Id,
                Name = department.Name,
                MinMembers = department.MinMembers,
                MaxMembers = department.MaxMembers,
                Description = department.Description,
            };

            dto.AffectedId = department.Id;

            return dto;
        }
    }
}
