using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.CourseSkill.Commands.CreateCourseSkill
{
    public record CreateCourseSkillCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int courseId,
        int skillId,
        int weight) : IRequest<ErrorOr<CoursesSkillsDto>>, IHeaderCarrier;
}
