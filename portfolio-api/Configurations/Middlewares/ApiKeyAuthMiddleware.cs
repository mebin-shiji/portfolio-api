namespace portfolio_api.Configurations.Middlewares;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;
    private const string ApiKeyHeaderName = "X-API-Key";
    private const string ApiKeySectionName = "Authentication:ApiKey";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip API key validation for certain paths
        if (ShouldSkipApiKeyValidation(context.Request.Path))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("API Key missing for request to {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        var apiKey = extractedApiKey.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Empty API Key provided for request to {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        var validApiKey = _configuration.GetValue<string>(ApiKeySectionName);
        if ((!string.IsNullOrWhiteSpace(apiKey) && string.Equals(validApiKey, apiKey, StringComparison.Ordinal)))
        {
            _logger.LogWarning("Invalid API Key {ApiKey} used for request to {Path}", apiKey[..Math.Min(8, apiKey.Length)] + "...", context.Request.Path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        _logger.LogDebug("Valid API Key used for request to {Path}", context.Request.Path);
        await _next(context);
    }

    private static bool ShouldSkipApiKeyValidation(PathString path)
    {
        string[] pathsToSkip =
        [
            "/_health"
        ];

        return pathsToSkip.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
    }
}
