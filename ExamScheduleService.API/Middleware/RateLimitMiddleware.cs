using ExamScheduleService.Application.Common;
using System.Collections.Concurrent;

namespace ExamScheduleService.API.Middleware;

/// <summary>
/// Provides a simple per-client in-memory rate limit.
/// </summary>
public sealed class RateLimitMiddleware
{
    private static readonly ConcurrentDictionary<string, ClientRequestCounter> Counters = new();

    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private readonly int _permitLimit;
    private readonly TimeSpan _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger.</param>
    public RateLimitMiddleware(RequestDelegate next,IConfiguration configuration,ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _permitLimit = configuration.GetValue("RateLimiting:PermitLimit", 100);
        _window = TimeSpan.FromSeconds(configuration.GetValue("RateLimiting:WindowSeconds", 60));
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var clientKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var counter = Counters.GetOrAdd(clientKey, _ => new ClientRequestCounter(DateTime.UtcNow));

        if (IsRateLimitExceeded(counter))
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientKey}.", clientKey);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Too many requests. Please try again later.")).ConfigureAwait(false);
            return;
        }

        await _next(context).ConfigureAwait(false);
    }

    private bool IsRateLimitExceeded(ClientRequestCounter counter)
    {
        lock (counter.SyncRoot)
        {
            var now = DateTime.UtcNow;
            if (now - counter.WindowStart >= _window)
            {
                counter.WindowStart = now;
                counter.RequestCount = 0;
            }

            if (counter.RequestCount >= _permitLimit)
            {
                return true;
            }

            counter.RequestCount++;
            return false;
        }
    }

    private sealed class ClientRequestCounter
    {
        public ClientRequestCounter(DateTime windowStart)
        {
            WindowStart = windowStart;
        }

        public DateTime WindowStart { get; set; }

        public int RequestCount { get; set; }

        public object SyncRoot { get; } = new();
    }
}
