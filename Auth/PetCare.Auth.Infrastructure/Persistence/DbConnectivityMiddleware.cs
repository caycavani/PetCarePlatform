using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetCare.Auth.Infrastructure.Persistence;
using System.Diagnostics;
using System.Net.Http;

public class DbConnectivityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DbConnectivityMiddleware> _logger;

    public DbConnectivityMiddleware(RequestDelegate next, ILogger<DbConnectivityMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AuthDbContext db)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var canConnect = await db.Database.CanConnectAsync();
            _logger.LogInformation("DB Connectivity: {Status} in {Elapsed}ms", canConnect, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DB Connectivity failed after {Elapsed}ms", stopwatch.ElapsedMilliseconds);
        }

        await _next(context);
    }
}
