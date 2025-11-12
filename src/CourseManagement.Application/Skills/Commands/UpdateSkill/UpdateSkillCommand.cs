using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Skills.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Skills.Commands.UpdateSkill
{
    public record UpdateSkillCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string oldSkillSkillName,
        string? skillName,
        int? levelCap) : IRequest<ErrorOr<SkillDto>>, IHeaderCarrier;
}
