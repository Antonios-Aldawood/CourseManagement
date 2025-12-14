using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace CourseManagement.Application.Courses.Commands.Validator
{
    public static class MaterialValidatorRules
    {
        public static void ApplyMaterialRules<T>(
            AbstractValidator<T> validator,
            Expression<Func<T, IFormFile>> getFile,
            Expression<Func<T, bool>> getIsVideo)
        {
            validator.When(x => !string.IsNullOrEmpty(getFile.Compile().Invoke(x).ToString()), () =>
            {
                validator.RuleFor(getFile)
                .Must(ValidFile)
                .WithMessage("File must not be over 100 MB size, and must have a name that is allowed by windows and between 1 and 90 characters.");
            }).Otherwise(() =>
            {
                validator.RuleFor(getFile)
                .NotEmpty()
                .WithMessage("File can't be empty.");
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

        private static bool ValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            if (file.Length > 100 * 1024 * 1024)
            {
                return false;
            }

            string pattern = @"[<>|*?\""]";
            if (file.FileName.Length < 1 || file.FileName.Length > 100 ||
                Regex.IsMatch(file.FileName, pattern))
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> ValidFileAsync(IFormFile file, string extension)
        {
            Dictionary<string, List<byte[]>> fileSignatures = new()
            {
                // PDF: %PDF-
                [".pdf"] = new List<byte[]>
                {
                    new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }
                },

                // MP4: ftyp
                [".mp4"] = new List<byte[]>
                {
                    new byte[] { 0x66, 0x74, 0x79, 0x70 } // "ftyp"
                }
            };

            if (fileSignatures.ContainsKey(extension) == false)
            {
                return false;
            }

            using Stream stream = file.OpenReadStream();

            // Read enough bytes for all known signatures
            int maxSignatureLength = fileSignatures[extension].Max(s => s.Length) + 4;
            byte[] buffer = new byte[maxSignatureLength];

            //await stream.ReadAsync(buffer, 0, buffer.Length);
            await stream.ReadExactlyAsync(buffer);

            foreach (byte[] signature in fileSignatures[extension])
            {
                if (extension == ".mp4")
                {
                    // MP4 signature is offset (ftyp starts at byte 4)
                    if (buffer.Skip(4).Take(signature.Length).SequenceEqual(signature))
                    {
                        return true;
                    }
                }
                else
                {
                    if (buffer.Take(signature.Length).SequenceEqual(signature))
                    {
                        return true;
                    }
                }
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

/*
private static bool ValidPath(string path)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        return false;
    }
    
    // Valid path characters
    if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
    {
        return false;
    }

    // Valid file characters
    char[] invalidFileChars = Path.GetInvalidFileNameChars();
    foreach (var part in path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
    {
        if (part.IndexOfAny(invalidFileChars) >= 0)
            return false;
    }

    return Path.IsPathRooted(path);
}

private static bool ValidPath(string path)
{
    if (!string.IsNullOrEmpty(path))
    {
        string criteria = @"^[\""<>|*?]";
        return
            path != null &&
            Regex.IsMatch(path, criteria)! &&
            Path.IsPathRooted(path);
    }

    return false;
}

private static bool ValidPath(string path)
{
    if (string.IsNullOrWhiteSpace(path))
        return false;

    // Reject if contains invalid chars
    char[] invalid = Path.GetInvalidPathChars();
    if (path.IndexOfAny(invalid) >= 0)
        return false;

    return Path.IsPathRooted(path);
}
*/

/*
        ////////// The working validator for path where entered instead of files //////////
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

        private static bool ValidPath(string path)
        {
            string pattern = @"[<>|*?\""]";

            if (Regex.IsMatch(path, pattern))
            {
                return false;
            }

            //return Path.IsPathRooted(path);
            return true;
        }
*/