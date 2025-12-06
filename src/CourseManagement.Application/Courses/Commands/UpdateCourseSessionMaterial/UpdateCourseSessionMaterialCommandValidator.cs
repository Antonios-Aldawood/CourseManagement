using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Courses.Commands.Validator;

namespace CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterial
{
    public class UpdateCourseSessionMaterialCommandValidator : AbstractValidator<UpdateCourseSessionMaterialCommand>
    {
        public UpdateCourseSessionMaterialCommandValidator()
        {
            MaterialValidatorRules.ApplyMaterialRules(
                validator: this,
                getFile: x => x.file,
                getIsVideo: x => x.isVideo);
        }
    }
}
