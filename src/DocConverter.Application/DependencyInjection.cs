using DocConverter.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using DocConverter.Application.Interfaces;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileService, FileService>();
        services.AddSingleton<IConversionQueue, ConversionQueue>();
        services.AddScoped<IConversionService, ConversionService>();

        return services;
    }
}