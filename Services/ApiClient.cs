using System.Text;
using RestApiClient.Models;

namespace RestApiClient.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly bool _verbose;
    private readonly string _baseAddress;
    private readonly AuthenticationSettings _authSettings;

    public ApiClient(string baseAddress, int timeoutSeconds, AuthenticationSettings authSettings, bool verbose = false)
    {
        _verbose = verbose;
        _baseAddress = baseAddress.TrimEnd('/');
        _authSettings = authSettings;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseAddress),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };
    }

    public async Task<string> SendRequestAsync(
        string method,
        string endpoint,
        Dictionary<string, string>? parameters = null,
        string? body = null,
        string? authToken = null,
        bool requiresAuth = true)
    {
        // Replace parameters in endpoint (e.g., "files/{fileId}" -> "files/123")
        var actualEndpoint = endpoint;
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                actualEndpoint = actualEndpoint.Replace($"{{{param.Key}}}", param.Value);
            }
        }

        var fullUrl = $"{_baseAddress}/{actualEndpoint.TrimStart('/')}";

        if (_verbose)
        {
            Console.WriteLine($"[VERBOSE] Sending {method} request to: {fullUrl}");
        }

        var request = new HttpRequestMessage(new HttpMethod(method.ToUpper()), actualEndpoint);

        // Add authentication token if required
        if (requiresAuth && !string.IsNullOrEmpty(authToken))
        {
            var authHeader = $"{_authSettings.TokenPrefix} {authToken}";
            request.Headers.Add(_authSettings.TokenHeaderName, authHeader);

            if (_verbose)
            {
                Console.WriteLine($"[VERBOSE] Added authentication header");
            }
        }

        // Add body for methods that support it
        if (!string.IsNullOrEmpty(body))
        {
            if (method.ToUpper() is "POST" or "PUT" or "PATCH")
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                
                if (_verbose)
                {
                    Console.WriteLine($"[VERBOSE] Request body: {body}");
                }
            }
        }

        try
        {
            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {responseBody}");
            }

            if (_verbose)
            {
                Console.WriteLine($"[VERBOSE] Response status: {(int)response.StatusCode} {response.StatusCode}");
            }

            return responseBody;
        }
        catch (TaskCanceledException ex)
        {
            throw new HttpRequestException($"Request timeout: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}