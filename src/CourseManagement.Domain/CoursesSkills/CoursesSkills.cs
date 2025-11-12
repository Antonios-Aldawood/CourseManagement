using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.CoursesSkills
{
    public class CoursesSkills
    {
        public int Id { get; init; }
        public int CourseId { get; private set; }
        public int SkillId { get; private set; }
        public int Weight { get; private set; } = 0;

        private CoursesSkills(
            int courseId,
            int skillId,
            int weight)
        {
            CourseId = courseId;
            SkillId = skillId;
            Weight = weight;
        }

        private ErrorOr<Success> CheckIfCoursesSkillsIsValid()
        {
            if (Weight < 1 ||
                Weight > 5)
            {
                return CoursesSkillsErrors.CoursesSkillsWeightOutOfBounds;
            }

            if (CourseId <= 0 ||
                SkillId <= 0)
            {
                return CoursesSkillsErrors.CourseSkillIdsAreInvalid;
            }

            return Result.Success;
        }

        private static ErrorOr<Success> CheckIfWeightIsValid(
            int weight,
            int courseId,
            int skillId)
        {
            if (weight < 1 ||
                weight > 5)
            {
                return CoursesSkillsErrors.CoursesSkillsWeightOutOfBounds;
            }

            if (courseId <= 0 ||
                skillId <= 0)
            {
                return CoursesSkillsErrors.CourseSkillIdsAreInvalid;
            }

            return Result.Success;
        }

        public static ErrorOr<CoursesSkills> CreateCourseSkill(
            int courseId,
            int skillId,
            int weight)
        {
            var courseSkill = new CoursesSkills(
                courseId: courseId,
                skillId: skillId,
                weight: weight);

            if (courseSkill.CheckIfCoursesSkillsIsValid() != Result.Success)
            {
                return courseSkill.CheckIfCoursesSkillsIsValid().Errors;
            }

            return courseSkill;
        }

        public ErrorOr<CoursesSkills> UpdateCourseSkill(
            int courseId,
            int skillId,
            int weight)
        {
            var courseSkillValidity = CheckIfWeightIsValid(
                courseId: courseId,
                skillId: skillId,
                weight: weight);

            if (courseSkillValidity != Result.Success)
            {
                return courseSkillValidity.Errors;
            }

            CourseId = courseId;
            SkillId = skillId;
            Weight = weight;

            return this;
        }
    }
}
