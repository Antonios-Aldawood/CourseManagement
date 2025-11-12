using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.JobsSkills
{
    public class JobsSkills
    {
        public int Id { get; init; }
        public int JobId { get; private set; }
        public int SkillId { get; private set; }
        public int Weight { get; private set; } = 0;

        private JobsSkills(
            int jobId,
            int skillId,
            int weight)
        {
            JobId = jobId;
            SkillId = skillId;
            Weight = weight;
        }

        private ErrorOr<Success> CheckIfJobsSkillsIsValid()
        {
            if (Weight < 1 ||
                Weight > 5)
            {
                return JobsSkillsErrors.JobsSkillsWeightOutOfBounds;
            }

            if (JobId <= 0 ||
                SkillId <= 0)
            {
                return JobsSkillsErrors.JobSkillIdsAreInvalid;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfWeightIsValid(
            int jobId,
            int skillId,
            int weight)
        {
            if (weight < 1 ||
                weight > 5)
            {
                return JobsSkillsErrors.JobsSkillsWeightOutOfBounds;
            }

            if (jobId <= 0 ||
                skillId <= 0)
            {
                return JobsSkillsErrors.JobSkillIdsAreInvalid;
            }

            return Result.Success;
        }

        public static ErrorOr<JobsSkills> CreateJobSkill(
            int jobId,
            int skillId,
            int weight)
        {
            var jobSkill = new JobsSkills(
                jobId: jobId,
                skillId: skillId,
                weight: weight);

            if (jobSkill.CheckIfJobsSkillsIsValid() != Result.Success)
            {
                return jobSkill.CheckIfJobsSkillsIsValid().Errors;
            }

            return jobSkill;
        }

        public ErrorOr<JobsSkills> UpdateJobSkill(
            int jobId,
            int skillId,
            int weight)
        {
            var jobSkillValidity = CheckIfWeightIsValid(
                jobId: jobId,
                skillId: skillId,
                weight: weight);

            if (jobSkillValidity != Result.Success)
            {
                return jobSkillValidity.Errors;
            }

            JobId = jobId;
            SkillId = skillId;
            Weight = weight;

            return this;
        }
    }
}
