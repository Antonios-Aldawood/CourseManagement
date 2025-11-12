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
    public static class SessionValidatorRules
    {
        public static void ApplySessionRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string>> getName,
            Expression<Func<T, DateTimeOffset>> getStartDate,
            Expression<Func<T, DateTimeOffset>> getEndDate)
        {
            validator.When(x => !string.IsNullOrEmpty(getName.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getName)
                .MinimumLength(5)
                .MaximumLength(50)
                .Must(ValidName)
                .WithMessage("Session name must be between 5 and 50 characters long, and only letters, numbers, or markers.");
            });

            validator.When(x => !string.IsNullOrEmpty(getStartDate.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getStartDate)
                .ExclusiveBetween(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5))
                .Must(ValidDateTimeOffset)
                .WithMessage("Start date either entered in wrong format, or somehow entered from the past, or beyond the foreseeable future.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getStartDate)
                    .NotEmpty().WithMessage("Start date is required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getEndDate.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getEndDate)
                .ExclusiveBetween(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5))
                .Must(ValidDateTimeOffset)
                .WithMessage("End date either entered in wrong format, or somehow entered from the past, or beyond the foreseeable future.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getEndDate)
                    .NotEmpty().WithMessage("End date is required.");
            });
        }

        private static bool ValidName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                string criteria = @"^[a-zA-Z0-9 \-,.]{5,50}$";
                return name != null && Regex.IsMatch(name, criteria);
            }

            return false;
        }

        private static bool ValidDateTimeOffset(DateTimeOffset date)
        {
            if (DateTimeOffset.Compare(DateTimeOffset.UtcNow, date) > 0)
            {
                return false;
            }

            return true;
        }
    }
}
