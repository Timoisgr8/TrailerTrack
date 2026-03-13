using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TrailerTrack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR - scans assembly for all handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Register all FluentValidation validators in this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}