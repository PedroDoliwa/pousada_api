using Microsoft.EntityFrameworkCore;
using PousadaApi.Api.Configurations;
using PousadaApi.Api.Middlewares;
using PousadaApi.Infrastructure.Data.Context;

string? dotEnvPath = null;
foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
{
    var dir = new DirectoryInfo(Path.GetFullPath(start));
    while (dir != null)
    {
        var candidate = Path.Combine(dir.FullName, ".env");
        if (File.Exists(candidate))
        {
            dotEnvPath = candidate;
            break;
        }
        dir = dir.Parent;
    }
    if (dotEnvPath != null)
        break;
}
if (dotEnvPath != null)
    DotNetEnv.Env.Load(dotEnvPath);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PousadaDbContext>();
        await db.Database.MigrateAsync();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("FrontDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
