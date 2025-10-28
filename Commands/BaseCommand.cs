using RestApiClient.Services;

namespace RestApiClient.Commands;

public abstract class BaseCommand
{
    protected readonly ApiClient ApiClient;
    protected readonly AuthenticationService AuthService;
    protected readonly bool Verbose;

    protected BaseCommand(ApiClient apiClient, AuthenticationService authService, bool verbose)
    {
        ApiClient = apiClient;
        AuthService = authService;
        Verbose = verbose;
    }

    public abstract Task<int> ExecuteAsync();
}
