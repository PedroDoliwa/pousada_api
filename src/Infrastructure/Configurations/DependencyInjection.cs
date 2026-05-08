using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Authentication;
using PousadaApi.Infrastructure.Data.Context;
using PousadaApi.Infrastructure.Data.Repositories;

namespace PousadaApi.Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<PousadaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPousadaRepository, PousadaRepository>();
        services.AddScoped<IQuartoRepository, QuartoRepository>();
        services.AddScoped<IHospedeRepository, HospedeRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, Sha256PasswordHasher>();

        return services;
    }
}
