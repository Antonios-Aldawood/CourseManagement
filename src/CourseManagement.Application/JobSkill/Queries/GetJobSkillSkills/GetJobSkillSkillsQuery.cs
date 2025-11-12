using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.JobSkill.Queries.GetJobSkillSkills
{
    public record GetJobSkillSkillsQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int jobId) : IRequest<ErrorOr<List<JobsSkillsDto>>>, IHeaderCarrier;
}
