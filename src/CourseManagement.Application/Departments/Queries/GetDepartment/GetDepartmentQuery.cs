using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Departments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Departments.Queries.GetDepartment
{
    public record GetDepartmentQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        string name) : IRequest<ErrorOr<List<DepartmentDto>>>, IHeaderCarrier;
}
