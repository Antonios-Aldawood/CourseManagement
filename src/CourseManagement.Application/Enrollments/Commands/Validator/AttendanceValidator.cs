using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;

namespace CourseManagement.Application.Enrollments.Commands.Validator
{
    public static class AttendanceValidator
    {
        public static void ApplyAttendanceRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, DateTimeOffset>> getDateAttended)
        {
            validator.When(x => !string.IsNullOrEmpty(getDateAttended.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getDateAttended)
                .LessThan(DateTimeOffset.UtcNow)
                .WithMessage("Date attended can not be in the future.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getDateAttended)
                .NotEmpty()
                .WithMessage("Date attended can not be empty.");
            });
        }
    }
}
