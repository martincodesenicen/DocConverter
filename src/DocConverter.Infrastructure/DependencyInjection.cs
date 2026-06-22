using DocConverter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocConverter.Infrastructure.Authentication;
using DocConverter.Domain.Interfaces;
using DocConverter.Application.Interfaces;
using DocConverter.Infrastructure.Repositories;

namespace DocConverter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("DocConverter.Infrastructure"))); // Las migraciones se van a guardar aca
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
}