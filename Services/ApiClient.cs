using System.Text;

namespace RestApiClient.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly bool _verbose;
    private readonly string _baseAddress;

    public ApiClient(string baseAddress, int timeoutSeconds, bool verbose = false)
    {
        _verbose = verbose;
        _baseAddress = baseAddress.TrimEnd('/');
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseAddress),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };
    }

    public async Task<string> SendRequestAsync(
        string method,
        string endpoint,
        Dictionary<string, string> parameters,
        string[] headers,
        string? body = null)
    {
        // Replace parameters in endpoint (e.g., "files/{fileId}" -> "files/123")
        var actualEndpoint = endpoint;
        foreach (var param in parameters)
        {
            actualEndpoint = actualEndpoint.Replace($"{{{param.Key}}}", param.Value);
        }

        var fullUrl = $"{_baseAddress}/{actualEndpoint.TrimStart('/')}";

        if (_verbose)
        {
            Console.WriteLine($"[VERBOSE] Sending {method} request to: {fullUrl}");
        }

        var request = new HttpRequestMessage(new HttpMethod(method.ToUpper()), actualEndpoint);

        // Add custom headers
        foreach (var header in headers)
        {
            var parts = header.Split(':', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                request.Headers.Add(key, value);

                if (_verbose)
                {
                    Console.WriteLine($"[VERBOSE] Header: {key} = {value}");
                }
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

            var result = $"Status: {(int)response.StatusCode} {response.StatusCode}\n";
            
            if (_verbose)
            {
                result += $"Full URL: {fullUrl}\n";
                result += $"Headers:\n";
                foreach (var header in response.Headers)
                {
                    result += $"  {header.Key}: {string.Join(", ", header.Value)}\n";
                }
                result += "\n";
            }

            result += $"Body:\n{responseBody}";

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed with status {response.StatusCode}");
            }

            return result;
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