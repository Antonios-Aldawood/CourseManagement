using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Skills.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Skills.Commands.CreateSkill
{
    public record CreateSkillCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string skillName,
        int levelCap,
        int courseId,
        int weight) : IRequest<ErrorOr<SkillDto>>, IHeaderCarrier;
}
