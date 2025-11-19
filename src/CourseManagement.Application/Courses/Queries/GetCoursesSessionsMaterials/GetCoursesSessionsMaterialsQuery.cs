using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Queries.GetCoursesSessionsMaterials
{
    public record GetCoursesSessionsMaterialsQuery(
        string ipAddress,
        Dictionary<string, string> headers) : IRequest<ErrorOr<List<CourseWithSessionsAndMaterialsDto>>>, IHeaderCarrier;
}
