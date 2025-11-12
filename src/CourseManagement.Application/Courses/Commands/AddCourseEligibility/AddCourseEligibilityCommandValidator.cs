using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Courses.Commands.Validator;

namespace CourseManagement.Application.Courses.Commands.AddCourseEligibility
{
    public class AddCourseEligibilityCommandValidator : AbstractValidator<AddCourseEligibilityCommand>
    {
        public AddCourseEligibilityCommandValidator()
        {
            EligibilityValidatorRules.ApplyEligibilityRules(
                validator: this,
                getPosition: x => x.position,
                getPositionIds: x => x.positionIds,
                getDepartment: x => x.department,
                getDepartmentIds: x => x.departmentIds,
                getJob: x => x.job,
                getJobIds: x => x.jobIds);
        }
    }
}

/*
            When(x => !string.IsNullOrEmpty(x.keys.ToString()), () =>
            {
                RuleFor(x => x.keys)
                .Must(ValidKeys)
                .WithMessage("Keys must be between 1 and 3, and must be either 'position', 'job', or 'department'.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.keys)
                    .NotEmpty().WithMessage("Keys cannot be completely empty.");
            });

            When(x => !string.IsNullOrEmpty(x.values.ToString()), () =>
            {
                RuleFor(x => x.values)
                .Must(ValidValues)
                .WithMessage("");
            });

        private bool ValidKeys(List<string> keys)
        {
            if (keys.Count < 0 || keys.Count > 3)
            {
                return false;
            }

            string criteria = @"^[A-Z][a-z]{3,10}$";

            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    return key != null && Regex.IsMatch(key, criteria) ? true : false;
                }
            }

            return false;
        }

        private bool ValidValues(List<List<int>> values)
        {
            if (values.Count < 0 || values.Count > 3)
            {
                return false;
            }

            foreach (List<int> innerValues in values)
            {
                foreach (int innerValue in innerValues)
                {
                    if (!string.IsNullOrEmpty(innerValue.ToString()))
                    {
                        string criteria = @"^[0-9]";
                        return innerValue.ToString() != null && Regex.IsMatch(innerValue.ToString(), criteria) ? true : false;
                    }
                }
            }

            return false;
        }
*/