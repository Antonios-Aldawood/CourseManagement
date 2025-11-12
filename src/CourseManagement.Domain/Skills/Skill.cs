using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Skills
{
    public class Skill
    {
        public int Id { get; init; }
        public string SkillName { get; private set; } = string.Empty;
        public int LevelCap { get; private set; } = 0;

        private Skill(
            string skillName,
            int levelCap)
        {
            SkillName = skillName;
            LevelCap = levelCap;
        }

        private ErrorOr<Success> CheckIfSkillIsValid()
        {
            if (LevelCap < 1 ||
                LevelCap > 10)
            {
                return SkillErrors.SkillLevelCapOutOfBounds;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfLevelCapIsValid(
            int levelCap)
        {
            if (levelCap < 1 ||
                levelCap > 10)
            {
                return SkillErrors.SkillLevelCapOutOfBounds;
            }

            return Result.Success;
        }

        public static ErrorOr<Skill> CreateSkill(
            string skillName,
            int levelCap)
        {
            var skill = new Skill(
                skillName: skillName,
                levelCap: levelCap);

            if (skill.CheckIfSkillIsValid() != Result.Success)
            {
                return skill.CheckIfSkillIsValid().Errors;
            }

            return skill;
        }

        public ErrorOr<Skill> UpdateSkill(
            string skillName,
            int levelCap)
        {
            var skillValidity = CheckIfLevelCapIsValid(
                levelCap: levelCap);

            if (skillValidity != Result.Success)
            {
                return skillValidity.Errors;
            }

            SkillName = skillName;
            LevelCap = levelCap;

            return this;
        }
    }
}
