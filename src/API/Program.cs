using API.Extensions;
using API.Middleware;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext().WriteTo.Console());

    builder.Services.AddControllers(o => o.Filters.Add<API.Filters.FluentValidationFilter>());
    builder.Services.AddScoped<API.Filters.FluentValidationFilter>();
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddApiVersioningAndSwagger();
    builder.Services.AddResponseCompression();
    builder.Services.AddCors(o => o.AddPolicy("AssessmentCors", p => p.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"]).AllowAnyHeader().AllowAnyMethod()));

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        await next();
    });
    app.UseHttpsRedirection();
    app.UseResponseCompression();
    app.UseCors("AssessmentCors");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow })).AllowAnonymous();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();
        await DbSeeder.SeedAsync(db, hasher);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally { Log.CloseAndFlush(); }

public partial class Program { }
