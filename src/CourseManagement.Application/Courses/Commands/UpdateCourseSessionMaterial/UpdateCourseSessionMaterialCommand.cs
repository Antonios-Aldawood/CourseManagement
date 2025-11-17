using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Commands.UpdateCourseSessionMaterial
{
    public record UpdateCourseSessionMaterialCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int courseId,
        int sessionId,
        int materialId,
        string path,
        bool isVideo) : IRequest<ErrorOr<SessionAndMaterialsDto>>, IHeaderCarrier;
}
