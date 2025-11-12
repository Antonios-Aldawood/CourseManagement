using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Courses.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Courses.Commands.AddCourseSession
{
    public record AddCourseSessionCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string subject,
        string sessionName,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        int trainerId,
        bool isOffline,
        int? seats,
        string? link,
        string? app) : IRequest<ErrorOr<SessionWithTrainerDto>>, IHeaderCarrier;
}
