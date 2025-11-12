using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Common.Authorization;
using CourseManagement.Application.Common.Behaviors;
using FluentValidation;

namespace CourseManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationService, AuthorizationService>();

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));

                options.AddOpenBehavior(typeof(LoggingBehavior<,>));

                options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));

                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

            return services;
        }
    }
}
