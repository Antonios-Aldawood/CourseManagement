using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace CourseManagement.Domain.Courses
{
    public class Eligibility
    {
        public int Id { get; set; }
        public int CourseId { get; set; } = 0;
        public string Key { get; init; } = string.Empty;
        public int Value { get; init; } = 0;

        private Eligibility(
            int courseId,
            string key,
            int value)
        {
            CourseId = courseId;
            Key = key;
            Value = value;
        }

        private ErrorOr<string> PositionEligibilityIdentifier(string[] positions)
        {
            if (Key != "Position")
            {
                return EligibilityErrors.EligibilityValueDoesNotBelongToPositionKey;
            }

            for (int i = 0; i < positions.Length; i++)
            {
                if (Value == i+1)
                {
                    return positions[i];
                }
            }

            return EligibilityErrors.PositionMismatch;
        }

        private ErrorOr<Success> CheckIfEligibilityIsValid(string[] positions)
        {
            if (Key != "Job" &&
                Key != "Position" &&
                Key != "Department")
            {
                return EligibilityErrors.KeyUnknown;
            }

            if (Key == "Position")
            {
                if (Value <= 0 || Value > 7)
                {
                    return EligibilityErrors.EligibilityValueDoesNotBelongToPositionKey;
                }

                if (PositionEligibilityIdentifier(positions).IsError)
                {
                    return PositionEligibilityIdentifier(positions).Errors;
                }
            }

            if (Value <= 0)
            {
                return EligibilityErrors.ValueBelowOrEqualToZero;
            }

            return Result.Success;
        }

        internal static ErrorOr<Eligibility> CreateEligibility(
            int courseId,
            string key,
            int value,
            string[] positions)
        {
            Eligibility eligibility = new Eligibility(
                courseId: courseId,
                key: key,
                value: value);

            if (eligibility.CheckIfEligibilityIsValid(positions) != Result.Success)
            {
                return eligibility.CheckIfEligibilityIsValid(positions).Errors;
            }

            return eligibility;
        }
    }
}

/*
        private ErrorOr<string> PositionEligibilityIdentifier(string[] positions)
        {
            if (Key != "Position")
            {
                return EligibilityErrors.EligibilityValueDoesNotBelongToPositionKey;
            }

            string valueName = string.Empty;

            switch (Value)
            {
                case 1:
                    valueName = positions[0];
                    return valueName;

                case 2:
                    valueName = positions[1];
                    return valueName;

                case 3:
                    valueName = positions[2];
                    return valueName;

                case 4:
                    valueName = positions[3];
                    return valueName;

                case 5:
                    valueName = positions[4];
                    return valueName;

                case 6:
                    valueName = positions[5];
                    return valueName;

                case 7:
                    valueName = positions[6];
                    return valueName;

                default:
                    return EligibilityErrors.PositionMismatch;
            }
        }
*/