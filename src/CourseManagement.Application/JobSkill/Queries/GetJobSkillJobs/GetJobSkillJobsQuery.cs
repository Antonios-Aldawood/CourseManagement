using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.JobSkill.Common.Dto;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.JobSkill.Queries.GetJobSkillJobs
{
    public record GetJobSkillJobsQuery(
        string ipAddress,
        Dictionary<string, string> headers,
        int skillId) : IRequest<ErrorOr<List<JobsSkillsDto>>>, IHeaderCarrier;
}
