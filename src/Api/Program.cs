using Microsoft.EntityFrameworkCore;
using PousadaApi.Api.Configurations;
using PousadaApi.Api.Middlewares;
using PousadaApi.Infrastructure.Data.Context;

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

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
