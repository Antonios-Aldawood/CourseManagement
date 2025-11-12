using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.JobSkill.Commands.Validator
{
    public static class JobSkillValidatorRules
    {
        public static void ApplyJobSkillRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, int>> getWeight)
        {
            validator.When(x => !string.IsNullOrEmpty(getWeight.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getWeight)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(25)
                .Must(ValidWeight)
                .WithMessage("Skill weight for this course is either too little or too high, must be between 1 and 5.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getWeight)
                    .NotEmpty().WithMessage("Skill weight for this course is required.");
            });
        }

        private static bool ValidWeight(int number)
        {
            if (!string.IsNullOrEmpty(number.ToString()))
            {
                string criteria = @"^[0-9]{1,2}$";
                return number.ToString() != null && Regex.IsMatch(number.ToString(), criteria) ? true : false;
            }

            return false;
        }
    }
}
