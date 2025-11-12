using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.CourseSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.CourseSkill.Commands.UpdateCourseSkillSkill
{
    public record UpdateCourseSkillSkillCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int oldSkillId,
        int? skillId,
        int? weight) : IRequest<ErrorOr<List<CoursesSkillsIdsDto>>>, IHeaderCarrier;
}
