using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Departments.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Departments.Queries.SortDepartmentsByLeastMembers
{
    public record SortDepartmentsByLeastMembersQuery(
        string ipAddress,
        Dictionary<string, string> headers) : IRequest<ErrorOr<List<DepartmentDto>>>, IHeaderCarrier;
}
