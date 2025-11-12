using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.CourseSkill.Commands.Validator;

namespace CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillCourse
{
    public class UpdateCourseSkillCourseCommandValidator : AbstractValidator<UpdateCourseSkillCourseCommand>
    {
        public UpdateCourseSkillCourseCommandValidator()
        {
            CourseSkillValidatorRules.ApplyCourseSkillRules(
                validator: this,
                getWeight: x => x.weight ?? 3);
        }
    }
}
