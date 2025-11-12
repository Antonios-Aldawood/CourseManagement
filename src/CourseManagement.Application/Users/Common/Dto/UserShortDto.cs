using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record UserShortDto : IHasAffectedIds
    {
        public required int Id { get; set; }
        public required string Alias { get; set; } = string.Empty;

        public int AffectedId { get; set; }
    }
}
