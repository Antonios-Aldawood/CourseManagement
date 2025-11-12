using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            When(x => !string.IsNullOrEmpty(x.alias), () =>
            {
                RuleFor(x => x.alias)
                .MinimumLength(3)
                .MaximumLength(75)
                .Must(ValidAlias)
                .WithMessage("Alias must be between 3 and 75 characters, without containing any special characters.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.alias)
                .NotEmpty()
                .WithMessage("Alias is required.");
            });

            When(x => !string.IsNullOrEmpty(x.email), () =>
            {
                RuleFor(x => x.email)
                .NotEmpty()
                .WithMessage("User's email can't be empty.")
                .MinimumLength(5)
                .WithMessage("User's email is too short.")
                .MaximumLength(35)
                .WithMessage("User's email is too long.")
                .Must(ValidEmail)
                .WithMessage("User's email is in invalid format, please use 'example@email.com' format.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.email)
                .NotEmpty()
                .WithMessage("User's email cannot be empty.");
            });

            When(x => !string.IsNullOrEmpty(x.password), () =>
            {
                RuleFor(x => x.password)
                .MinimumLength(8)
                .WithMessage("User's password cannot be less than 8 characters.")
                .MaximumLength(40)
                .WithMessage("User's password cannot be more than 40 characters.")
                .Must(ValidPassword)
                .WithMessage("User's password isn't valid, please use at least one number and at least one arithmetic character.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.password)
                .NotEmpty()
                .WithMessage("User's password cannot be empty.");
            });

            When(x => !string.IsNullOrEmpty(x.phoneNumber), () =>
            {
                RuleFor(x => x.phoneNumber)
                .Length(10)
                .Must(ValidPhoneNumber)
                .WithMessage("User's phone number must have 10 digits using only numbers, starting with 09.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.phoneNumber)
                .NotEmpty()
                .WithMessage("User's phone number required.");
            });

            When(x => !string.IsNullOrEmpty(x.position), () =>
            {
                RuleFor(x => x.position)
                .MinimumLength(3)
                .MaximumLength(50)
                .Must(ValidPosition)
                .WithMessage("Position must be between 3 and 50 characters, without containing any special characters. Please enter either 'CEO', 'Director', 'Manager', 'Supervisor', 'Specialist', or 'Intern.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.position)
                .NotEmpty()
                .WithMessage("Position is required.");
            });

            When(x => !string.IsNullOrEmpty(x.city), () =>
            {
                RuleFor(x => x.city)
                .MinimumLength(3)
                .MaximumLength(25)
                .Must(ValidAddressString)
                .WithMessage("Address's city must be between 3 and 25 characters, and not containing any special characters, beginning with a Cap letter.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.city)
                .NotEmpty()
                .WithMessage("Address's city is required.");
            });

            When(x => !string.IsNullOrEmpty(x.region), () =>
            {
                RuleFor(x => x.region)
                .MinimumLength(3)
                .MaximumLength(25)
                .Must(ValidAddressString)
                .WithMessage("Address's region must be between 3 and 25 characters, and not containing any special characters, beginning with a Cap letter.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.region)
                .NotEmpty()
                .WithMessage("Address's region is required.");
            });

            When(x => !string.IsNullOrEmpty(x.road), () =>
            {
                RuleFor(x => x.road)
                .MinimumLength(3)
                .MaximumLength(25)
                .Must(ValidAddressString)
                .WithMessage("Address's road must be between 3 and 25 characters, and not containing any special characters, beginning with a Cap letter.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.road)
                .NotEmpty()
                .WithMessage("Address's road is required.");
            });

            When(x => !string.IsNullOrEmpty(x.agreedSalary.ToString()), () =>
            {
                RuleFor(x => x.agreedSalary)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(500000)
                .Must(ValidSalaryNumber)
                .WithMessage("User's agreed salary either too little or too high, must be between the bounds of the user job's salary, and with only two decimal digits.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.agreedSalary)
                .NotEmpty()
                .WithMessage("User's amount of agreed salary is required.");
            });
        }

        private bool ValidAlias(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                string criteria = @"^[a-zA-Z]{3,75}$";
                return alias != null && Regex.IsMatch(alias, criteria) ? true : false;
            }

            return false;
        }

        private bool ValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                string criteria = @"^[a-zA-Z0-9,._% +-]+@[a-zA-Z0-9,-]+\.[a-zA-Z]{2,}$";
                return email != null && Regex.IsMatch(email, criteria) ? true : false;
            }

            return false;
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

        private bool ValidPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string criteria = @"^09+[0-9]{8}$";
                return phoneNumber != null && Regex.IsMatch(phoneNumber, criteria) ? true : false;
            }

            return false;
        }

        private bool ValidPosition(string pos)
        {
            if (!string.IsNullOrEmpty(pos))
            {
                string criteria = @"^[a-zA-Z ]{3,75}$";
                return pos != null && Regex.IsMatch(pos, criteria) ? true : false;
            }

            return false;
        }

        private bool ValidAddressString(string addressString)
        {
            if (!string.IsNullOrEmpty(addressString))
            {
                string criteria = @"^[A-Z][a-z ]{3,25}$";
                return addressString != null && Regex.IsMatch(addressString, criteria) ? true : false;
            }

            return false;
        }

        private bool ValidSalaryNumber(double salary)
        {
            double multiplied = salary * 100;
            return Math.Abs(multiplied - Math.Round(multiplied)) < 1e-9;
        }
    }
}
