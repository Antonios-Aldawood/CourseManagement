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

namespace CourseManagement.Application.Departments.Queries.SortDepartmentsByMostMembers
{
    public class SortDepartmentsByMostMembersQueryHandler(
        IDepartmentsRepository departmentsRepository
        ) : IRequestHandler<SortDepartmentsByMostMembersQuery, ErrorOr<List<DepartmentDto>>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;

        public async Task<ErrorOr<List<DepartmentDto>>> Handle(SortDepartmentsByMostMembersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var sortedDepartments = await _departmentsRepository.SortByMostMembersAsync();

                if (sortedDepartments is not List<Department> departments ||
                    sortedDepartments.Count == 0)
                {
                    return Error.Validation(description: "Departments not found.");
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
