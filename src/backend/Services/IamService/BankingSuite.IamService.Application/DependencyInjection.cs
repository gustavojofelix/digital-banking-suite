using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Later: add pipeline behaviors (validation, logging, transactions)
        return services;
    }
}