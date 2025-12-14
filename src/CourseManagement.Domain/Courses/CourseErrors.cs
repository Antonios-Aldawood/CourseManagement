using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class CourseErrors
    {
        public static readonly Error CourseSubjectIsNotValid = Error.Validation(
            "Course.CourseSubjectIsNotValid",
            "Course subject should be between 3 and 35 characters long.");

        public static readonly Error EligibilityAlreadyGivenToCourse = Error.Validation(
            "Course.EligibilityAlreadyGivenToCourse",
            "Course already has this eligibility.");

        public static readonly Error SessionAlreadyAssignedToCourse = Error.Validation(
            "Course.SessionAlreadyAssignedToCourse",
            "Course already has this session assigned to it.");

        public static readonly Error SessionTimeFrameConflictsWithOtherSessions = Error.Validation(
            "Course.SessionTimeFrameConflictsWithOtherSessions",
            "Session timeframe conflicts with other sessions.");

        public static readonly Error CourseSessionsReachedMaximumAmount = Error.Validation(
            "Course.CourseSessionsReachedMaximumAmount",
            "Course can not have more than 12 sessions.");

        public static readonly Error SessionNotFound = Error.Validation(
            "Course.SessionNotFound",
            "Course does not have session.");

        //public static readonly Error SkillAlreadyExisting = Error.Validation(
        //    "Course.SkillAlreadyExisting",
        //    "Course already has skill with the same skill-name.");

        //public static readonly Error SkillDoesNotExist = Error.Validation(
        //    "Course.SkillDoesNotExist",
        //    "Skill hasn't been found.");

        //public static readonly Error CourseAlreadyHasSkill = Error.Validation(
        //    "Course.CourseAlreadyHasSkill",
        //    "Course already has skill.");

        //public static readonly Error CourseDoesNotHaveSkill = Error.Validation(
        //    "Course.CourseDoesNotHaveSkill",
        //    "Course does not have said skill.");
    }
}
