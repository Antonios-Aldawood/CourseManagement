using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Courses.Commands.Validator;
using FluentValidation;

namespace CourseManagement.Application.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            CourseValidatorRules.ApplyCourseRules(
                validator: this,
                getSubject: x => x.subject ?? "Valid subject",
                getDescription: x => x.description ?? "Valid description.");
        }
    }
}
