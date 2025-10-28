using System.Text.Json;
using RestApiClient.Services;

namespace RestApiClient.Commands;

public class DownloadFileCommand : BaseCommand
{
    private readonly string _fileId;
    private readonly bool _full;
    private readonly string? _output;

    public DownloadFileCommand(
        ApiClient apiClient,
        AuthenticationService authService,
        bool verbose,
        string fileId,
        bool full,
        string? output)
        : base(apiClient, authService, verbose)
    {
        _fileId = fileId;
        _full = full;
        _output = output;
    }

    public override async Task<int> ExecuteAsync()
    {
        try
        {
            if (Verbose)
            {
                Console.WriteLine("[VERBOSE] Starting download-file command...");
            }

            // Authenticate and get token
            if (Verbose)
            {
                Console.WriteLine("[VERBOSE] Authenticating...");
            }
            
            var token = await AuthService.GetAuthenticationTokenAsync();
            
            if (Verbose)
            {
                Console.WriteLine("[VERBOSE] Authentication successful");
            }

            // Determine endpoint based on --full flag
            string endpoint = _full 
                ? $"files/{{fileId}}/full" 
                : $"files/{{fileId}}";

            var parameters = new Dictionary<string, string>
            {
                ["fileId"] = _fileId
            };

            // Send request to get file data
            if (Verbose)
            {
                Console.WriteLine($"[VERBOSE] Downloading file {_fileId} (full: {_full})");
            }

            var response = await ApiClient.SendRequestAsync(
                method: "GET",
                endpoint: endpoint,
                parameters: parameters,
                authToken: token
            );

            // Save to file or output to console
            if (!string.IsNullOrEmpty(_output))
            {
                await File.WriteAllTextAsync(_output, response);
                Console.WriteLine($"File saved to: {_output}");
            }
            else
            {
                Console.WriteLine(response);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            if (Verbose)
            {
                Console.Error.WriteLine($"Details: {ex}");
            }
            return 1;
        }
    }
}
