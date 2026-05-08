using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PousadaApi.Infrastructure.Data.Context;

/// <summary>
/// Permite executar <c>dotnet ef</c> com <c>--project src/Infrastructure --startup-project src/Api</c>
/// carregando a connection string de <c>src/Api/appsettings.json</c>.
/// </summary>
public sealed class PousadaDbContextFactory : IDesignTimeDbContextFactory<PousadaDbContext>
{
    public PousadaDbContext CreateDbContext(string[] args)
    {
        var apiProjectDirectory = ResolveApiProjectDirectory();

        var repoEnv = Path.Combine(Path.GetFullPath(Path.Combine(apiProjectDirectory, "..", "..")), ".env");
        if (File.Exists(repoEnv))
            DotNetEnv.Env.Load(repoEnv);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<PousadaDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PousadaDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiProjectDirectory()
    {
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "..", "Api"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Api"),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Api"),
        };

        foreach (var relative in candidates)
        {
            var full = Path.GetFullPath(relative);
            if (File.Exists(Path.Combine(full, "appsettings.json")) &&
                File.Exists(Path.Combine(full, "PousadaApi.csproj")))
            {
                return full;
            }
        }

        throw new InvalidOperationException(
            "Não foi possível localizar src/Api (appsettings.json + PousadaApi.csproj). " +
            "Execute a partir da pasta da solução, por exemplo: dotnet ef database update --project src/Infrastructure --startup-project src/Api");
    }
}
