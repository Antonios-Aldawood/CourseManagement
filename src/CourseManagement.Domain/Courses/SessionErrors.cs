using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class SessionErrors
    {
        public static readonly Error StartTimeOrEndTimeBeyondBoundaries = Error.Validation(
            "Session.StartTimeOrEndTimeBeyondBoundaries",
            "Start-time or end-time beyond months boundaries.");

        public static readonly Error StartTimeCannotBeAfterEndTime = Error.Validation(
            "Session.StartTimeCannotBeAfterEndTime",
            "Start-time cannot be after end-time.");

        public static readonly Error EndTimeCannotBeMoreThan10HoursAfterStartTime = Error.Validation(
            "Session.EndTimeCannotBeMoreThan10HoursAfterStartTime",
            "End-time cannot be more than 10 hours after start-time.");

        public static readonly Error SeatsExceedingLimit = Error.Validation(
            "Session.SeatsExceedingLimit",
            "Session seats can not cross the 50 limit.");

        public static readonly Error MaterialAlreadyGivenToSession = Error.Validation(
            "Session.MaterialAlreadyGivenToSession",
            "Session already has this material.");

        public static readonly Error SessionCanNotHaveMoreThanFourMaterials = Error.Validation(
            "Session.SessionCanNotHaveMoreThanFourMaterials",
            "Session can't have more than four materials.");

        public static readonly Error SessionMaterialNotFound = Error.Validation(
            "Session.SessionMaterialNotFound",
            "Session material not found.");
    }
}
