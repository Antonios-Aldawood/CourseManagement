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

namespace CourseManagement.Application.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler(
        IDepartmentsRepository departmentsRepository
        ) : IRequestHandler<GetAllDepartmentsQuery, ErrorOr<List<DepartmentDto>>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;

        public async Task<ErrorOr<List<DepartmentDto>>> Handle(GetAllDepartmentsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<Department> departments = await _departmentsRepository.GetAllDepartmentsAsync();
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
