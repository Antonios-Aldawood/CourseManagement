using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CourseManagement.Application;
using CourseManagement.Infrastructure;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
{
    // For static files to not be hosted and accessed before authorization and controllers are accessed. 
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