using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Users.Common.Dto
{
    public record UserJobSkillsDto : IHasAffectedIds
    {
        public required int UserId { get; set; }
        public required string Alias { get; set; }
        public required string Position { get; set; }
        public required int JobId { get; set; }
        public required string JobTitle { get; set; }
        public required int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public required string SkillName { get; set; }
        public required int Weight { get; set; }

        public int AffectedId { get; set; }

        //public UserJobSkillsDto(
        //    int userId,
        //    string alias,
        //    string email,
        //    string jobTitle,
        //    string departmentName,
        //    List<string> skills,
        //    List<int> weights)
        //{
        //    UserId = userId;
        //    Alias = alias;
        //    Email = email;
        //    JobTitle = jobTitle;
        //    DepartmentName = departmentName;
        //    JobSkillsAndWeights.Add(key: skills.)
        //}
    }
}
