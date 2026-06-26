using DocConverter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocConverter.Infrastructure.Authentication;
using DocConverter.Domain.Interfaces;
using DocConverter.Application.Interfaces;
using DocConverter.Infrastructure.Repositories;
using DocConverter.Infrastructure.Storage;
using DocConverter.Infrastructure.Services;
using DocConverter.Infrastructure.BackgroundJobs;
using DocConverter.Infrastructure.Converters;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DocConverter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("DocConverter.Infrastructure"))); // Las migraciones se van a guardar aca
        
        services.AddHttpContextAccessor();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();
        services.AddScoped<IContentConverter, PdfProcessingEngine>();
        services.AddHostedService<ConversionBackgroundWorker>();
        services.AddScoped<IConversionJobRepository, ConversionJobRepository>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();

        return services;
    }
}