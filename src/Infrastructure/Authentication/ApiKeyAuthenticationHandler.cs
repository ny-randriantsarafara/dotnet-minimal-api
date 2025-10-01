using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> Options, ILoggerFactory Logger, UrlEncoder Encoder) : base(Options, Logger, Encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        HttpRequest request = Context.Request;
        HttpResponse response = Context.Response;

        if (!request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("API Key was not provided."));
        }

        string apiKey = apiKeyHeaderValues.FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals("my-secret-api-key"))
        {
            response.StatusCode = 403;
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "API Key") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}