using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Users;

namespace CourseManagement.Application.Users.Common.Dto
{
    public class UserJobAndDepartmentDto : IHasAffectedIds
    {
        public required User User { get; set; }
        public required User Upper1 { get; set; }
        public required int JobId { get; set; }
        public required string Job { get; set; }
        public required int DepartmentId { get; set; }
        public required string Department { get; set; }

        public int AffectedId { get; set; }
    }
}
