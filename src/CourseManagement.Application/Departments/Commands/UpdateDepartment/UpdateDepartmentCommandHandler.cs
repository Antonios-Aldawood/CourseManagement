using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Departments.Common.Dto;
using CourseManagement.Domain.Departments;

namespace CourseManagement.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler(
        IDepartmentsRepository departmentsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateDepartmentCommand, ErrorOr<DepartmentDto>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<DepartmentDto>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var oldDepartment = await _departmentsRepository.GetByExactNameAsync(command.oldDepartmentName);

                if (oldDepartment == null)
                {
                    return Error.Validation(description: "Department does not exist.");
                }

                if (command.name != null &&
                    command.name != oldDepartment.Name &&
                    await _departmentsRepository.NameExistsAsync(command.name))
                {
                    return Error.Validation(description: "Department name already taken.");
                }

                var newDepartment = oldDepartment.UpdateDepartment(
                    name: command.name ?? oldDepartment.Name,
                    minMembers: command.minMembers ?? oldDepartment.MinMembers,
                    maxMembers: command.maxMembers ?? oldDepartment.MaxMembers,
                    description: command.description ?? oldDepartment.Description);

                if (newDepartment.IsError)
                {
                    return newDepartment.Errors;
                }

                _departmentsRepository.UpdateDepartment(oldDepartment, newDepartment.Value);

                await _unitOfWork.CommitChangesAsync();

                DepartmentDto dto = DepartmentDto.AddDto(newDepartment.Value);

                return dto;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
