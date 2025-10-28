namespace RestApiClient.Models;

public class ApiSettings
{
    public string BaseAddress { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}

public class AuthenticationSettings
{
    public string LoginEndpoint { get; set; } = "auth/login";
    public string TokenHeaderName { get; set; } = "Authorization";
    public string TokenPrefix { get; set; } = "Bearer";
}

public class AppConfiguration
{
    public ApiSettings ApiSettings { get; set; } = new();
    public AuthenticationSettings Authentication { get; set; } = new();
}
