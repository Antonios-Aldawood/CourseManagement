using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace CourseManagement.Application.Common.Interfaces
{
    public interface IRabbitMqConnection
    {
        IConnection Connection { get; }
        internal Task InitializeAsync();
    }
}
