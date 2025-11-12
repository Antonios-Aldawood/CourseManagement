using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CourseManagement.Application.Departments.Commands.Validator;

namespace CourseManagement.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            DepartmentValidatorRules.ApplyDepartmentRules(
                validator: this,
                getName: x => x.name ?? "Valid name",
                getMinMembers: x => x.minMembers ?? 50,
                getMaxMembers: x => x.maxMembers ?? 50,
                getDescription: x => x.description ?? "Valid description"
            );
        }
    }
}
