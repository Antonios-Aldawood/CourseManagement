using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using RabbitMQ.Client;

namespace CourseManagement.Application.Common.Brokers
{
    public class RabbitMqProducer : IMessageProducer
    {
        private readonly IRabbitMqConnection _connection;

        public RabbitMqProducer(IRabbitMqConnection connection)
        {
            _connection = connection;
        }

        public async Task SendMessage<T>(T message)
        {
            await using var channel = await _connection.Connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                "eligibilities",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "eligibilities",
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }
    }
}
