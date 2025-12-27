using CourseManagement.Application;
using CourseManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.Configure<StaticFileOptions>(_ => { });

        builder.Services.AddControllers()
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddProblemDetails();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                //options.RequireHttpsMetadata = true;
                //options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                    ValidateIssuerSigningKey = true
                };
            });

        builder.Services
            .AddApplication()
            .AddInfrastructure();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisURL"];
            options.InstanceName = "CourseManagement";
        });

        //var MyAllowSpecificOrigins = "http://localhost:13657/";

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });
    }

    var app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.Use(async (context, next) =>
        {
            foreach (var header in context.Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                Console.WriteLine($"Authorization Header Received: {authHeader}");
            }
            else
            {
                Console.WriteLine("Authorization Header Not Found");
            }
            await next();
        });

        app.UseCors();
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
catch (Exception ex)
{
    Console.WriteLine(
        ex.GetType().Name + "\n" +
        ex.Message + "\n" +
        ex.InnerException + "\n" +
        ex.ToString());
}

/*
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

app.Use(async (context, next) =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost"
    };

    var connection = await factory.CreateConnectionAsync();

    using var channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(
        "eligibilities",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    var consumer = new AsyncEventingBasicConsumer(channel);

    consumer.ReceivedAsync += async (sender, args) =>
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"Message received: {message}");

        await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(args.DeliveryTag, multiple: false);
    };

    await channel.BasicConsumeAsync(
        queue: "eligibilities",
        autoAck: true,
        consumer: consumer);

    await next();
});
*/
