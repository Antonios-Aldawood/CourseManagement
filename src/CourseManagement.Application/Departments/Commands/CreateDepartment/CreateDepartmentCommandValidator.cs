using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Departments.Commands.Validator;

namespace CourseManagement.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            DepartmentValidatorRules.ApplyDepartmentRules(
                validator: this,
                getName: x => x.name,
                getMinMembers: x => x.minMembers,
                getMaxMembers: x => x.maxMembers,
                getDescription: x => x.description
            );
        }
    }
}
