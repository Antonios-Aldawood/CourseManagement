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

namespace CourseManagement.Application.Departments.Queries.GetDepartment
{
    public class GetDepartmentQueryHandler(
        IDepartmentsRepository departmentsRepository
        ) : IRequestHandler<GetDepartmentQuery, ErrorOr<List<DepartmentDto>>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;

        public async Task<ErrorOr<List<DepartmentDto>>> Handle(GetDepartmentQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var foundDepartments = await _departmentsRepository.GetByNameAsync(query.name);

                if (foundDepartments is not List<Department> departments ||
                    foundDepartments.Count == 0)
                {
                    return Error.Validation(description: "Department not found.");
                }

                List<DepartmentDto> departmentResponse = [];

                foreach (Department department in departments)
                {
                    DepartmentDto dto = DepartmentDto.AddDto(department);

                    departmentResponse.Add(dto);
                }

                return departmentResponse;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
