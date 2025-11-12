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

namespace CourseManagement.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler(
        IDepartmentsRepository departmentsRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<CreateDepartmentCommand, ErrorOr<DepartmentDto>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<DepartmentDto>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (await _departmentsRepository.NameExistsAsync(command.name) == true)
                {
                    return Error.Validation(description: "Department name already taken.");
                }

                var department = Department.CreateDepartment(
                    name: command.name,
                    minMembers: command.minMembers,
                    maxMembers: command.maxMembers,
                    description: command.description);

                if (department.IsError)
                {
                    return department.Errors;
                }

                await _departmentsRepository.AddDepartmentAsync(department.Value);

                await _unitOfWork.CommitChangesAsync();

                DepartmentDto dto = DepartmentDto.AddDto(department.Value);

                return dto;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
