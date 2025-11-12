using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.CoursesSkills
{
    public class CoursesSkillsErrors
    {
        public static readonly Error CoursesSkillsWeightOutOfBounds = Error.Validation(
            "CoursesSkills.CoursesSkillsWeightOutOfBounds",
            "Course's skill can't have weight higher than 5 nor lower than 1.");

        public static readonly Error CourseSkillIdsAreInvalid = Error.Validation(
            "CoursesSkills.CourseSkillIdsAreInvalid",
            "Course or skill has invalid id.");
    }
}
