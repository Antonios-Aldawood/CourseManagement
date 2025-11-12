using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.JobSkill.Commands.UpdateJobSkillSkill
{
    public record UpdateJobSkillSkillCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        int oldSkillId,
        int? skillId,
        int? weight) : IRequest<ErrorOr<List<JobsSkillsDto>>>, IHeaderCarrier;
}
