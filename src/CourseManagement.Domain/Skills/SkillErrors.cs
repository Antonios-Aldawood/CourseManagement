using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Skills
{
    public class SkillErrors
    {
        public static readonly Error SkillLevelCapOutOfBounds = Error.Validation(
            "Skill.SkillLevelCapOutOfBounds",
            "Skill's level cap is out of range, please enter a level cap between 1 and 10.");
    }
}
