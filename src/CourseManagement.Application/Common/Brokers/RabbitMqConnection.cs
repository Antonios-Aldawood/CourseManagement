using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using RabbitMQ.Client;

namespace CourseManagement.Application.Common.Brokers
{
    public class RabbitMqConnection : IRabbitMqConnection, IAsyncDisposable
    {
        private IConnection? _connection;
        public IConnection Connection => _connection!;

        private readonly ConnectionFactory _factory;

        public RabbitMqConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }

        public async Task InitializeAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
                await _connection.DisposeAsync();
        }
    }
}

/*
public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private IConnection? _connection;
    public IConnection Connection => _connection!;

    public RabbitMqConnection()
    {
        InitializeConnection();
    }

    private async void InitializeConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };

        _connection = await factory.CreateConnectionAsync();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
*/