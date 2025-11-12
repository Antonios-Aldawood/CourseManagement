using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Courses.Commands.Validator;

namespace CourseManagement.Application.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            CourseValidatorRules.ApplyCourseRules(
                validator: this,
                getSubject: x => x.subject,
                getDescription: x => x.description);
        }
    }
}
