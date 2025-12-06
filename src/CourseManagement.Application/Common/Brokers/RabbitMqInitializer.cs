using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CourseManagement.Application.Common.Brokers
{
    public class RabbitMqInitializer : IHostedService
    {
        private readonly IRabbitMqConnection _connection;

        public RabbitMqInitializer(IRabbitMqConnection connection)
        {
            _connection = connection;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _connection.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
