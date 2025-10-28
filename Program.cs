using System.CommandLine;
using System.CommandLine.Invocation;
using RestApiClient.Commands;
using RestApiClient.Services;

// Load configuration
var configService = new ConfigurationService();
var apiSettings = configService.ApiSettings;
var authSettings = configService.Authentication;

// Initialize services
var apiClient = new ApiClient(
    baseAddress: apiSettings.BaseAddress,
    timeoutSeconds: apiSettings.TimeoutSeconds,
    authSettings: authSettings,
    verbose: false
);

var authService = new AuthenticationService(configService, apiClient);

// Create root command
var rootCommand = new RootCommand("REST API Client - Preconfigured CLI tool");

// Global verbose option
var verboseOption = new Option<bool>(
    new[] { "--verbose", "-v" },
    "Enable verbose output"
);
rootCommand.AddGlobalOption(verboseOption);

// download-file command
var downloadFileCommand = new Command("download-file", "Download a file from the API");

var fileIdArgument = new Argument<string>(
    "file-id",
    "File ID to download"
);
downloadFileCommand.AddArgument(fileIdArgument);

var fullOption = new Option<bool>(
    new[] { "--full", "-F" },
    "Download full file information"
);
downloadFileCommand.Add(fullOption);

var outputOption = new Option<string?>(
    new[] { "--output", "-o" },
    "Output file path"
);
downloadFileCommand.Add(outputOption);

downloadFileCommand.SetHandler(async (InvocationContext ctx) =>
{
    var verbose = ctx.ParseResult.GetValueForOption(verboseOption);
    var fileId = ctx.ParseResult.GetValueForArgument(fileIdArgument);
    var full = ctx.ParseResult.GetValueForOption(fullOption);
    var output = ctx.ParseResult.GetValueForOption(outputOption);

    // Update API client verbosity
    apiClient = new ApiClient(
        baseAddress: apiSettings.BaseAddress,
        timeoutSeconds: apiSettings.TimeoutSeconds,
        authSettings: authSettings,
        verbose: verbose
    );
    
    authService = new AuthenticationService(configService, apiClient);

    var command = new DownloadFileCommand(
        apiClient,
        authService,
        verbose,
        fileId,
        full,
        output
    );

    var exitCode = await command.ExecuteAsync();
    Environment.Exit(exitCode);
});

rootCommand.Add(downloadFileCommand);

// Add more commands here following the same pattern...
// Example:
// var uploadFileCommand = new Command("upload-file", "Upload a file to the API");
// rootCommand.Add(uploadFileCommand);

await rootCommand.InvokeAsync(args);

// Cleanup
apiClient.Dispose();