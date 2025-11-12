using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Domain.Skills;

namespace CourseManagement.Application.Skills.Common.Dto
{
    public record SkillSoleDto : IHasAffectedIds
    {
        public required int SkillId { get; set; }
        public required string SkillName { get; set; }
        public required int LevelCap { get; set; }
        
        public int AffectedId { get; set; }

        public static SkillSoleDto AddDto(Skill skill)
        {
            SkillSoleDto dto = new SkillSoleDto
            {
                SkillId = skill.Id,
                SkillName = skill.SkillName,
                LevelCap = skill.LevelCap
            };

            dto.AffectedId = skill.Id;

            return dto;
        }
    }
}
