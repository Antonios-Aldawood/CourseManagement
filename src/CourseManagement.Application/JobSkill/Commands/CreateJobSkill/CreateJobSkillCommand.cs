using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.JobSkill.Commands.CreateJobSkill
{
    public record CreateJobSkillCommand(
        string ipAddress,
        Dictionary<string, string> headers,
        string jobTitle,
        string skillSkillName,
        int weight) : IRequest<ErrorOr<JobsSkillsDto>>, IHeaderCarrier;
}
