using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Skills.Commands.Validator
{
    public static class SkillValidatorRules
    {
        public static void ApplySkillRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string>> getSkillName,
            Expression<Func<T, int>> getLevelCap)
        {
            validator.When(x => !string.IsNullOrEmpty(getSkillName.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getSkillName)
                .MinimumLength(3)
                .MaximumLength(30)
                .Must(ValidSkillName)
                .WithMessage("Skill name either too high, or too low, must be between 3 and 30 characters, and starts with a Cap letter without containing any special characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getSkillName)
                    .NotEmpty().WithMessage("Skill name is required");
            });

            validator.When(x => !string.IsNullOrEmpty(getLevelCap.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getLevelCap)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(50)
                .Must(ValidLevelCap)
                .WithMessage("Skill level cap is either too little or too high, must be between 1 and 5.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getLevelCap)
                    .NotEmpty().WithMessage("Skill level cap is required.");
            });
        }

        private static bool ValidSkillName(string skillName)
        {
            if (!string.IsNullOrEmpty(skillName))
            {
                string criteria = @"^[A-Z][a-z. ]{2,30}$";
                return skillName != null && Regex.IsMatch(skillName, criteria) ? true : false;
            }

            return false;
        }

        private static bool ValidLevelCap(int number)
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
