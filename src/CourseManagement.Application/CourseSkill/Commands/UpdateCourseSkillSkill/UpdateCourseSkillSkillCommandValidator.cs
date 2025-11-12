using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.CourseSkill.Commands.Validator;

namespace CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillSkill
{
    public class UpdateCourseSkillSkillCommandValidator : AbstractValidator<UpdateCourseSkillSkillCommand>
    {
        public UpdateCourseSkillSkillCommandValidator()
        {
            CourseSkillValidatorRules.ApplyCourseSkillRules(
                validator: this,
                getWeight: x => x.weight ?? 3);
        }
    }
}
