using Microsoft.Extensions.Configuration;
using RestApiClient.Models;

namespace RestApiClient.Services;

public class ConfigurationService
{
    private readonly AppConfiguration _config;

    public ConfigurationService()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        _config = new AppConfiguration();
        configuration.GetSection("ApiSettings").Bind(_config.ApiSettings);
        configuration.GetSection("Authentication").Bind(_config.Authentication);
    }

    public ApiSettings ApiSettings => _config.ApiSettings;
    public AuthenticationSettings Authentication => _config.Authentication;
}
