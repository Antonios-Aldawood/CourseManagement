using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.CourseSkill.Commands.Validator;
using FluentValidation;

namespace CourseManagement.Application.CourseSkill.Commands.CreateCourseSkill
{
    public class CreateCourseSkillCommandValidator : AbstractValidator<CreateCourseSkillCommand>
    {
        public CreateCourseSkillCommandValidator()
        {
            CourseSkillValidatorRules.ApplyCourseSkillRules(
                validator: this,
                getWeight: x => x.weight);
        }
    }
}
