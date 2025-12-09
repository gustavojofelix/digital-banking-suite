using System.Reflection;
using System.Text;
using BankingSuite.IamService.Application.Auth;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.Infrastructure.Email;
using BankingSuite.IamService.Infrastructure.Persistence;
using BankingSuite.IamService.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace BankingSuite.IamService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIamInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database
        var connectionString = configuration.GetConnectionString("IamDatabase");

        services.AddDbContext<IamDbContext>(
            (sp, options) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();

                if (env.IsEnvironment("IntegrationTests"))
                {
                    // 👉 Use InMemory provider when running integration tests
                    options.UseInMemoryDatabase("iam_integration_tests_db");
                }
                else
                {
                    options.UseNpgsql(connectionString);
                }
            }
        );

        // Identity
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IamDbContext>()
            .AddDefaultTokenProviders() // <- important for email + password tokens
            .AddSignInManager<SignInManager<ApplicationUser>>();

        // JWT options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions =
            configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.Key);

        // Authentication + JWT bearer
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

        // Authorization
        services.AddAuthorization();

        // JWT service
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailSender, LoggingEmailSender>();

        // MediatR - scan IAM Application assembly
        var applicationAssembly = Assembly.Load("BankingSuite.IamService.Application");

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        return services;
    }
}
