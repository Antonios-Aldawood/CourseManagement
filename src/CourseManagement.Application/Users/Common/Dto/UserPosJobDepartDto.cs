using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record UserPosJobDepartDto : IHasAffectedIds
    {
        public required int UserId { get; set; }
        public required string Alias { get; set; }
        public required string Email { get; set; }
        public required string Position { get; set; }
        public required int JobId { get; set; }
        public required string JobTitle { get; set; }
        public required int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }

        public int AffectedId { get; set; }
    }
}
