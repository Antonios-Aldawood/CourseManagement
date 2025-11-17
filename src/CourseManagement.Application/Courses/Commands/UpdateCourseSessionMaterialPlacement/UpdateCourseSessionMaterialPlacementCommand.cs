using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterialPlacement
{
    public record UpdateCourseSessionMaterialPlacementCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int oldMaterialId,
        int oldSessionId,
        int oldCourseId,
        string newMaterialSessionName,
        int? newCourseId) : IRequest<ErrorOr<SessionAndMaterialsDto>>, IHeaderCarrier;
}
