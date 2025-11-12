using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Roles.Commands.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            When(x => !string.IsNullOrEmpty(x.roleType), () =>
            {
                RuleFor(x => x.roleType)
                .MinimumLength(3)
                .MaximumLength(25)
                .Must(ValidRoleType)
                .WithMessage("Role must be between 3 and 25 characters, and starts with a Cap letter. Please enter either 'Admin', 'Trainer', or 'Trainee'.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.roleType)
                .NotEmpty()
                .WithMessage("Role is required.");
            });
        }

        private bool ValidRoleType(string roleType)
        {
            if (!string.IsNullOrEmpty(roleType))
            {
                string criteria = @"^[A-Za-z]{3,25}$";
                return roleType != null && Regex.IsMatch(roleType, criteria) ? true : false;
            }

            return false;
        }
    }
}
