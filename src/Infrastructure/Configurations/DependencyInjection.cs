using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Authentication;
using PousadaApi.Infrastructure.Data.Context;
using PousadaApi.Infrastructure.Data.Repositories;
using PousadaApi.Infrastructure.Integrations;
using PousadaApi.Infrastructure.Options;

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
        services.AddScoped<ICalendarioExternoRepository, CalendarioExternoRepository>();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, Sha256PasswordHasher>();

        services.AddSingleton<IIcalParser, IcalNetParser>();
        services.AddHttpClient<IIcalFeedClient, IcalFeedHttpClient>(client =>
        {
            var timeoutSeconds = configuration.GetValue("CalendarioSync:TimeoutSegundos", 60);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        services.AddOptions<OpenAiOptions>()
            .Bind(configuration.GetSection(OpenAiOptions.SectionName))
            .PostConfigure(options =>
            {
                if (string.IsNullOrWhiteSpace(options.ApiKey))
                    options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
            });

        return services;
    }
}
