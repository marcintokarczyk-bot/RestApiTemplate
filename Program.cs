using System.CommandLine;
using System.CommandLine.Invocation;
using RestApiClient.Services;

var rootCommand = new RootCommand("REST API Client - A CLI tool for making HTTP requests");

var baseAddressOption = new Option<string>(
    new[] { "--base", "-b" },
    "Base address of the API"
);
rootCommand.Add(baseAddressOption);

var methodOption = new Option<string>(
    new[] { "--method", "-m" },
    () => "GET",
    "HTTP method (GET, POST, PUT, DELETE, PATCH)"
);
rootCommand.Add(methodOption);

var endpointArgument = new Argument<string>(
    "endpoint",
    "The endpoint name to call (use --list to see available endpoints)"
) { Arity = ArgumentArity.ZeroOrOne };
rootCommand.Add(endpointArgument);

var paramOption = new Option<string[]>(
    new[] { "--param", "-p" },
    "Endpoint parameters (format: key=value)"
);
rootCommand.Add(paramOption);

var listOption = new Option<bool>(
    new[] { "--list", "-l" },
    "List available endpoints"
);
rootCommand.Add(listOption);

var headersOption = new Option<string[]>(
    new[] { "--header", "-H" },
    "Additional headers (format: Key:Value)"
);
rootCommand.Add(headersOption);

var bodyOption = new Option<string>(
    new[] { "--body", "--data", "-d" },
    "Request body for POST/PUT requests"
);
rootCommand.Add(bodyOption);

var timeoutOption = new Option<int>(
    new[] { "--timeout", "-t" },
    () => 30,
    "Request timeout in seconds"
);
rootCommand.Add(timeoutOption);

var outputOption = new Option<string>(
    new[] { "--output", "-o" },
    "Output file path for response"
);
rootCommand.Add(outputOption);

var verboseOption = new Option<bool>(
    new[] { "--verbose", "-v" },
    "Enable verbose output"
);
rootCommand.Add(verboseOption);

rootCommand.SetHandler(async (InvocationContext ctx) =>
{
    var list = ctx.ParseResult.GetValueForOption(listOption);
    
    try
    {
        // Show available endpoints if --list is specified
        if (list)
        {
            ApiEndpoints.PrintAvailableEndpoints();
            return;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
    
    try
    {
        var baseAddress = ctx.ParseResult.GetValueForOption(baseAddressOption);
        var method = ctx.ParseResult.GetValueForOption(methodOption);
        var endpoint = ctx.ParseResult.GetValueForArgument(endpointArgument);
        var parameters = ctx.ParseResult.GetValueForOption(paramOption);
        var headers = ctx.ParseResult.GetValueForOption(headersOption);
        var body = ctx.ParseResult.GetValueForOption(bodyOption);
        var timeout = ctx.ParseResult.GetValueForOption(timeoutOption);
        var output = ctx.ParseResult.GetValueForOption(outputOption);
        var verbose = ctx.ParseResult.GetValueForOption(verboseOption);

        // Check if baseAddress is provided
        if (string.IsNullOrEmpty(baseAddress))
        {
            Console.Error.WriteLine("Error: --base is required");
            Environment.Exit(1);
            return;
        }

        // Check if endpoint is provided
        if (endpoint == null)
        {
            Console.Error.WriteLine("Error: Endpoint is required (use --list to see available endpoints)");
            Environment.Exit(1);
            return;
        }

        // Validate endpoint
        if (!ApiEndpoints.IsEndpointValid(endpoint))
        {
            Console.Error.WriteLine($"Error: Endpoint '{endpoint}' not found");
            Console.Error.WriteLine("Use --list to see available endpoints");
            Environment.Exit(1);
            return;
        }

        // Get endpoint path
        var endpointPath = ApiEndpoints.GetEndpointPath(endpoint);

        // Parse parameters
        var parsedParams = new Dictionary<string, string>();
        string[]? paramArray = parameters;
        if (paramArray != null)
        {
            foreach (string param in paramArray)
            {
                var parts = param.Split('=', 2);
                if (parts.Length == 2)
                {
                    parsedParams[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        // Create API client
        var apiClient = new ApiClient(baseAddress, timeout, verbose);

        // Send request
        var response = await apiClient.SendRequestAsync(
            method: method ?? "GET",
            endpoint: endpointPath,
            parameters: parsedParams,
            headers: headers ?? Array.Empty<string>(),
            body: body
        );

        // Output response
        if (!string.IsNullOrEmpty(output))
        {
            await File.WriteAllTextAsync(output, response);
            Console.WriteLine($"Response saved to: {output}");
        }
        else
        {
            Console.WriteLine(response);
        }
    }
    catch (HttpRequestException ex)
    {
        Console.Error.WriteLine($"HTTP Error: {ex.Message}");
        Environment.Exit(1);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
});

await rootCommand.InvokeAsync(args);