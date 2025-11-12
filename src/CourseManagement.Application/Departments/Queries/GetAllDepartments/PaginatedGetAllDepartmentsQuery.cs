using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Departments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Common.Dto;

namespace CourseManagement.Application.Departments.Queries.GetAllDepartments
{
    public record PaginatedGetAllDepartmentsQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int pageNumber = 1,
        int pageSize = 10) : IRequest<ErrorOr<PagedResult<DepartmentDto>>>, IHeaderCarrier;
}
