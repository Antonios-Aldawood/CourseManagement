using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Courses.Commands.Validator
{
    public static class EligibilityValidatorRules
    {
        public static void ApplyEligibilityRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string?>> getPosition,
            Expression<Func<T, List<int>?>> getPositionIds,
            Expression<Func<T, string?>> getDepartment,
            Expression<Func<T, List<int>?>> getDepartmentIds,
            Expression<Func<T, string?>> getJob,
            Expression<Func<T, List<int>?>> getJobIds)
        {
            validator.When(x => getPosition.Compile().Invoke(x) != null, () =>
            {
                validator.RuleFor(getPosition)
                .Must(ValidKey)
                .WithMessage("Position key needs to be called 'Position'");
            });

            validator.When(x => getPositionIds.Compile().Invoke(x) != null, () =>
            {
                validator.RuleFor(getPositionIds)
                .Must(ValidIdsList)
                .WithMessage("Position values must be numbers, and belong to known positions.");
            });

            validator.When(x => getDepartment.Compile().Invoke(x) != null, () =>
            {
                validator.RuleFor(getDepartment)
                .Must(ValidKey)
                .WithMessage("Department key needs to be called 'Department'");
            });

            validator.When(x => getDepartmentIds.Compile().Invoke(x) != null, () =>
            {
                validator.RuleFor(getDepartmentIds)
                .Must(ValidIdsList)
                .WithMessage("Department values must be numbers, and belong to known departments.");
            });

            validator.When(x => getJob.Compile() != null, () =>
            {
                validator.RuleFor(getJob)
                .Must(ValidKey)
                .WithMessage("Job key needs to be called 'Job'");
            });

            validator.When(x => getJobIds.Compile().Invoke(x) != null, () =>
            {
                validator.RuleFor(getJobIds)
                .Must(ValidIdsList)
                .WithMessage("Job values must be numbers, and belong to known jobs.");
            });
        }

        private static bool ValidKey(string? key)
        {
            string criteria = @"^[A-Z][a-z]{2,11}$";
            return key != null && Regex.IsMatch(key, criteria) ? true : false;
        }

        private static bool ValidIdsList(List<int>? ids)
        {
            string criteria = @"^[0-9]";

            if (ids != null)
            {
                foreach (int id in ids)
                {
                    return id.ToString() != null && Regex.IsMatch(id.ToString(), criteria) ? true : false;
                }
            }

            return false;
        }
    }
}
