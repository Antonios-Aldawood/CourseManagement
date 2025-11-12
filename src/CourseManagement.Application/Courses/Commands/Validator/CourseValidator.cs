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
    public static class CourseValidatorRules
    {
        public static void ApplyCourseRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string>> getSubject,
            Expression<Func<T, string>> getDescription)
        {
            validator.When(x => !string.IsNullOrEmpty(getSubject.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getSubject)
                .MinimumLength(3)
                .MaximumLength(35)
                .Must(ValidName)
                .WithMessage("Course subject either too short or too high, must be between 3 and 25 characters, and starts with a Cap letter without containing any special characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getSubject)
                    .NotEmpty().WithMessage("Course subject is required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getDescription.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getDescription)
                .MinimumLength(10)
                .MaximumLength(300)
                .Must(ValidDescription)
                .WithMessage("Course description either too short or too high, must be between 10 and 300 characters, and do not contain any special characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getDescription)
                    .NotEmpty().WithMessage("Course description is required.");
            });
        }

        private static bool ValidName(string subject)
        {
            if (!string.IsNullOrEmpty(subject))
            {
                string criteria = @"^[A-Z][a-z. ]{3,75}$";
                return subject != null && Regex.IsMatch(subject, criteria) ? true : false;
            }

            return false;
        }

        private static bool ValidDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                string criteria = @"^[a-zA-Z0-9.' ,]{10,300}$";
                return description != null && Regex.IsMatch(description, criteria) ? true : false;
            }

            return false;
        }
    }
}
