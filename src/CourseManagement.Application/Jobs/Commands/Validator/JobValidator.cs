using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
//using System.Globalization;

namespace CourseManagement.Application.Jobs.Commands.Validator
{
    public static class JobValidatorRules
    {
        public static void ApplyJobRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string>> getTitle,
            Expression<Func<T, double>> getMinSalary,
            Expression<Func<T, double>> getMaxSalary,
            Expression<Func<T, string>> getDescription)
        {

            validator.When(x => !string.IsNullOrEmpty(getTitle.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getTitle)
                .MinimumLength(3)
                .MaximumLength(75)
                .Must(ValidTitle)
                .WithMessage("Job title either too short or too high, must be between 3 and 75 characters, and starts with a Cap letter without containing any special characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getTitle)
                    .NotEmpty().WithMessage("Job title is required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getMinSalary.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getMinSalary)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(500000)
                .Must(ValidSalaryNumber)
                .WithMessage("Job minimum amount of salary is either too little or too high, must be between 20000 and 150000 with only two decimal digits, and less than the job's maximum amount of salary.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getMinSalary)
                    .NotEmpty().WithMessage("Job minimum amount of salary is required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getMaxSalary.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getMaxSalary)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(500000)
                .Must(ValidSalaryNumber)
                .WithMessage("Job maximum amount of salary is either too little or too high, must be between 20000 and 150000 with only two decimal digits, and more than the job's minimum amount of salary.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getMaxSalary)
                    .NotEmpty().WithMessage("Job maximum amount of salary is required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getDescription.Compile().Invoke(x)), () =>
            {
                validator.RuleFor(getDescription)
                .MinimumLength(10)
                .MaximumLength(300)
                .Must(ValidDescription)
                .WithMessage("Job description either too short or too high, must be between 10 and 300 characters, and do not contain any special characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getDescription)
                    .NotEmpty().WithMessage("Job description is required.");
            });
        }

        private static bool ValidTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                string criteria = @"^[A-Z][a-z. ]{2,75}$";
                return title != null && Regex.IsMatch(title, criteria) ? true : false;
            }

            return false;
        }

        private static bool ValidSalaryNumber(double salaryNumber)
        {
            /// We had to fix ToString() a bit more than elsewhere, because it's not consistent with how it treats double types, and only one way is how we need it to treat them.
            //string formattedSalaryNumber = salaryNumber.ToString("F2", CultureInfo.InvariantCulture);
            //string criteria = @"^[0-9]{5,6}\.[0-9]{2}$";
            //return Regex.IsMatch(formattedSalaryNumber, criteria);

            double multiplied = salaryNumber * 100;
            return Math.Abs(multiplied - Math.Round(multiplied)) < 1e-9;
        }

        private static bool ValidDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                string criteria = @"^[A-Z]+[a-zA-Z0-9. ,]{10,300}$";
                return description != null && Regex.IsMatch(description, criteria) ? true : false;
            }

            return false;
        }
    }
}
