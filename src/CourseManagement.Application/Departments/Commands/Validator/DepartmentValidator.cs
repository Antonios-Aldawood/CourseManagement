using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Departments.Commands.Validator
{
    public static class DepartmentValidatorRules
    {
        public static void ApplyDepartmentRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string>> getName,
            Expression<Func<T, int>> getMinMembers,
            Expression<Func<T, int>> getMaxMembers,
            Expression<Func<T, string>> getDescription)
        {
            validator.When(x => !string.IsNullOrEmpty(getName.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getName)
                    .MinimumLength(3)
                    .MaximumLength(75)
                    .Must(ValidName)
                    .WithMessage("Department name either too short or too high, must be between 3 and 75 characters, and starts with a Cap letter without containing any special characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getName)
                    .NotEmpty().WithMessage("Department name is required.");
            });

            validator.When(x => getMinMembers.Compile().Invoke(x) != default, () =>
            {
                validator.RuleFor(getMinMembers)
                    .InclusiveBetween(0, 200)
                    .Must(ValidMembersNumber)
                    .WithMessage("Department minimum members must be between 20 and 75.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getMinMembers)
                    .NotEmpty().WithMessage("Minimum members required.");
            });

            validator.When(x => getMaxMembers.Compile().Invoke(x) != default, () =>
            {
                validator.RuleFor(getMaxMembers)
                    .InclusiveBetween(0, 200)
                    .Must(ValidMembersNumber)
                    .WithMessage("Department maximum members must be between 20 and 75.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getMaxMembers)
                    .NotEmpty().WithMessage("Maximum members required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getDescription.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getDescription)
                    .MinimumLength(10)
                    .MaximumLength(300)
                    .Must(ValidDescription)
                    .WithMessage("Description invalid.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getDescription)
                    .NotEmpty().WithMessage("Description is required.");
            });
        }

        private static bool ValidName(string name)
        {
            return !string.IsNullOrEmpty(name) &&
                   Regex.IsMatch(name, @"^[A-Z][a-z. ]{2,75}$");
        }

        private static bool ValidMembersNumber(int number)
        {
            return Regex.IsMatch(number.ToString(), @"^[0-9]{2}$");
        }

        private static bool ValidDescription(string description)
        {
            return !string.IsNullOrEmpty(description) &&
                   Regex.IsMatch(description, @"^[a-zA-Z0-9.' ,]{10,300}$");
        }
    }
}