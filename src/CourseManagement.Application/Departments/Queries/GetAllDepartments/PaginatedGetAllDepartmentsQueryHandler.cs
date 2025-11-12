using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Dto;
using CourseManagement.Application.Departments.Common.Dto;

namespace CourseManagement.Application.Departments.Queries.GetAllDepartments
{
    public class PaginatedGetAllDepartmentsQueryHandler(
        IDepartmentsRepository departmentsRepository
        ) : IRequestHandler<PaginatedGetAllDepartmentsQuery, ErrorOr<PagedResult<DepartmentDto>>>
    {
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;

        public async Task<ErrorOr<PagedResult<DepartmentDto>>> Handle(PaginatedGetAllDepartmentsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var (departments, totalCount) = await _departmentsRepository
                    .GetAllPaginatedDepartmentsAsync(query.pageNumber, query.pageSize);

                var departmentResponse = departments
                    .Select(DepartmentDto.AddDto)
                    .ToList();

                return new PagedResult<DepartmentDto>
                {
                    Items = departmentResponse,
                    PageNumber = query.pageNumber,
                    PageSize = query.pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
