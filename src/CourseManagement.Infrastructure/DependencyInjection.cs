using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CourseManagement.Infrastructure.Common.Persistence;
using CourseManagement.Infrastructure.Logging.Persistence;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Infrastructure.Departments.Persistence;
using CourseManagement.Infrastructure.Jobs.Persistence;
using CourseManagement.Infrastructure.Users.Persistence;
using CourseManagement.Infrastructure.Roles.Persistence;
using CourseManagement.Infrastructure.Courses.Persistence;
using CourseManagement.Infrastructure.Skills.Persistence;
using CourseManagement.Infrastructure.CourseSkill.Persistence;
using CourseManagement.Infrastructure.JobSkill.Persistence;
using CourseManagement.Infrastructure.Enrollments.Persistence;
using CourseManagement.Infrastructure.RefreshTokens.Persistence;
using CourseManagement.Infrastructure.Logs.Persistence;
using CourseManagement.Infrastructure.Email;

namespace CourseManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services
                .AddPersistence();
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddDbContext<CourseManagementDbContext>(options =>
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=DDDCMS.Db;Trusted_Connection=True;TrustServerCertificate=True;", x =>
                {
                    x.MigrationsHistoryTable("__EFMigrationsHistory_Course");
                }));
            services.AddDbContext<LoggingDbContext>(options =>
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=DDDCMS.Db;Trusted_Connection=True;TrustServerCertificate=True;", x =>
                {
                    x.MigrationsHistoryTable("__EFMigrationsHistory_Logs");
                }));
            services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
            services.AddScoped<IJobsRepository, JobsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<ICoursesRepository, CoursesRepository>();
            services.AddScoped<ISkillsRepository, SkillsRepository>();
            services.AddScoped<ICoursesSkillsRepository, CoursesSkillsRepository>();
            services.AddScoped<IJobsSkillsRepository, JobsSkillsRepository>();
            services.AddScoped<IEnrollmentsRepository, EnrollmentsRepository>();
            services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
            services.AddScoped<ILogsRepository, LogsRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CourseManagementDbContext>());
            services.AddScoped<ILogUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<LoggingDbContext>());

            return services;
        }
    }
}
