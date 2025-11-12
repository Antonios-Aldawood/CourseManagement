using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Users.Commands.RefreshUserToken
{
    public class RefreshUserTokenCommandValidator : AbstractValidator<RefreshUserTokenCommand>
    {
        public RefreshUserTokenCommandValidator()
        {
            When(x => !string.IsNullOrEmpty(x.refreshToken), () =>
            {
                RuleFor(x => x.refreshToken)
                .MinimumLength(20)
                .MaximumLength(160)
                .Must(ValidRefreshTokenChars)
                .WithMessage("Refresh token formatted wrongly.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.refreshToken)
                .NotEmpty()
                .WithMessage("Refresh token can't be empty nor null.");
            });
        }

        private bool ValidRefreshTokenChars(string refreshToken)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                string criteria = @"^[A-Za-z0-9+/=]{20,160}$";
                return refreshToken != null && Regex.IsMatch(refreshToken, criteria) ? true : false;
            }

            return false;
        }
    }
}
