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
    public static class MaterialValidatorRules
    {
        public static void ApplyMaterialRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, string>> getPath,
            Expression<Func<T, bool>> getIsVideo)
        {
            validator.When(x => !string.IsNullOrEmpty(getPath.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getPath)
                .Must(ValidPath)
                .WithMessage("Path must be rooted, and without any of the characters: \" \\ / < > | * ?");
            }).Otherwise(() =>
            {
                validator.RuleFor(getPath)
                .NotEmpty()
                .WithMessage("Path is required.");
            });

            validator.When(x => !string.IsNullOrEmpty(getIsVideo.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getIsVideo)
                .Must(ValidIsVideo)
                .WithMessage("Material is video can only be a boolean value of true or false.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getIsVideo)
                .NotEmpty()
                .WithMessage("Is video must be ascertained.");
            });
        }

        private static bool ValidPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string criteria = @"^[/\""<>|*?]";
                return
                    path != null &&
                    Regex.IsMatch(path, criteria)! &&
                    Path.IsPathRooted(path);
            }

            return false;
        }

        private static bool ValidIsVideo(bool isVideo)
        {
            if (!string.IsNullOrEmpty(isVideo.ToString()))
            {
                if (isVideo.ToString().ToLower() == "true" ||
                    isVideo.ToString().ToLower() == "false")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
