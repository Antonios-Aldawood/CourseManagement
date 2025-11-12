using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;
using CourseManagement.Application.Logs.Entity;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface ILogsRepository
    {
        Task<ErrorOr<Success>> AddLogAsync(Log log);
        Task<Log?> GetByIdAsync(int logId);
        Task<List<Log>> GetAllLogsAsync();
    }
}
