using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Users.Commands.VerifyUser
{
    public class VerifyUserCommandValidator : AbstractValidator<VerifyUserCommand>
    {
        public VerifyUserCommandValidator()
        {
            When(x => !string.IsNullOrEmpty(x.newPassword), () =>
            {
                RuleFor(x => x.newPassword)
                .MinimumLength(8)
                .WithMessage("User's password cannot be less than 8 characters.")
                .MaximumLength(40)
                .WithMessage("User's password cannot be more than 40 characters.")
                .Must(ValidPassword)
                .WithMessage("User's password isn't valid, please use at least one number and at least one arithmetic character.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.newPassword)
                .NotEmpty()
                .WithMessage("User's password cannot be empty.");
            });

            When(x => !string.IsNullOrEmpty(x.verificationCode), () =>
            {
                RuleFor(x => x.verificationCode)
                .Length(6)
                .Must(ValidVerificationCode)
                .WithMessage("User's verification code isn't valid, please enter a six char code that was sent to your email.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.verificationCode)
                .NotEmpty()
                .WithMessage("Verification code can't be empty.");
            });
        }

        private bool ValidPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                string criteria = @"^[a-zA-Z0-9,_*/ +-]{8,40}$";
                return password != null && Regex.IsMatch(password, criteria) ? true : false;
            }

            return false;
        }

        private bool ValidVerificationCode(string verificationCode)
        {
            if (!string.IsNullOrEmpty(verificationCode))
            {
                string criteria = @"^[A-Z0-9]{6}$";
                return verificationCode != null && Regex.IsMatch(verificationCode, criteria) ? true : false;
            }

            return false;
        }
    }
}
