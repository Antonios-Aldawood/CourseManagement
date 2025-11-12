using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.JobsSkills
{
    public class JobsSkillsErrors
    {
        public static readonly Error JobsSkillsWeightOutOfBounds = Error.Validation(
            "JobsSkills.JobsSkillsWeightOutOfBounds",
            "Job's skill can't have weight higher than 5 nor lower than 1.");

        public static readonly Error JobSkillIdsAreInvalid = Error.Validation(
            "JobsSkills.JobSkillIdsAreInvalid",
            "Job or skill has invalid id.");
    }
}
