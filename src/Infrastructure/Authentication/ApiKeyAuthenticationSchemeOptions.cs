using Microsoft.AspNetCore.Authentication;

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKeyHeaderName { get; set; }

}