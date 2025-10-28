using System.Text;
using System.Text.Json;
using RestApiClient.Models;

namespace RestApiClient.Services;

public class AuthenticationService
{
    private readonly ConfigurationService _configService;
    private readonly ApiClient _apiClient;
    private string? _cachedToken;

    public AuthenticationService(ConfigurationService configService, ApiClient apiClient)
    {
        _configService = configService;
        _apiClient = apiClient;
    }

    public async Task<string> GetAuthenticationTokenAsync()
    {
        // Return cached token if available
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            return _cachedToken;
        }

        var authSettings = _configService.Authentication;
        var apiSettings = _configService.ApiSettings;

        // Create login request body
        var loginBody = JsonSerializer.Serialize(new
        {
            username = apiSettings.Login,
            password = apiSettings.Password
        });

        try
        {
            // Send login request
            var response = await _apiClient.SendRequestAsync(
                method: "POST",
                endpoint: authSettings.LoginEndpoint,
                body: loginBody,
                requiresAuth: false
            );

            // Parse response to get token
            // Adjust this based on your API response format
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            
            // Common token response formats:
            // { "token": "..." }
            // { "accessToken": "..." }
            // { "data": { "token": "..." } }
            
            if (root.TryGetProperty("token", out var token))
            {
                _cachedToken = token.GetString() ?? string.Empty;
            }
            else if (root.TryGetProperty("accessToken", out var accessToken))
            {
                _cachedToken = accessToken.GetString() ?? string.Empty;
            }
            else if (root.TryGetProperty("data", out var data) && data.TryGetProperty("token", out var dataToken))
            {
                _cachedToken = dataToken.GetString() ?? string.Empty;
            }
            else
            {
                throw new Exception("Could not find token in authentication response. Expected 'token', 'accessToken', or 'data.token' field.");
            }

            if (string.IsNullOrEmpty(_cachedToken))
            {
                throw new Exception("Authentication token is empty");
            }

            return _cachedToken;
        }
        catch (Exception ex)
        {
            throw new Exception($"Authentication failed: {ex.Message}", ex);
        }
    }

    public void ClearCache()
    {
        _cachedToken = null;
    }
}
