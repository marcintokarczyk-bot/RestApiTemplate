# REST API Client CLI - Template

CLI narzędzie w C# do pracy z REST API z automatyczną autentykacją i predefiniowanymi komendami.

## Funkcje

- ✅ Konfiguracja przez plik `appsettings.json`
- ✅ Automatyczna autentykacja (logowanie i token)
- ✅ Predefiniowane komendy z własnymi opcjami
- ✅ Template do łatwej rozbudowy o nowe komendy
- ✅ Verbose output
- ✅ Base address i endpointy skonfigurowane w kodzie

## Instalacja

```bash
# Zainstaluj .NET SDK 8.0 jeśli nie masz
dotnet restore
dotnet build
```

## Konfiguracja

Edytuj plik `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseAddress": "https://api.example.com",
    "Login": "your-username",
    "Password": "your-password",
    "TimeoutSeconds": 30
  },
  "Authentication": {
    "LoginEndpoint": "auth/login",
    "TokenHeaderName": "Authorization",
    "TokenPrefix": "Bearer"
  }
}
```

### Parametry konfiguracji:

- **ApiSettings.BaseAddress** - Bazowy adres API
- **ApiSettings.Login** - Nazwa użytkownika do logowania
- **ApiSettings.Password** - Hasło do logowania
- **ApiSettings.TimeoutSeconds** - Timeout w sekundach
- **Authentication.LoginEndpoint** - Endpoint do logowania (względny do BaseAddress)
- **Authentication.TokenHeaderName** - Nazwa nagłówka z tokenem (domyślnie: "Authorization")
- **Authentication.TokenPrefix** - Prefix tokenu (domyślnie: "Bearer")

## Użycie

### Podstawowe komendy

```bash
# Lista dostępnych komend
dotnet run -- --help

# Pomoc dla konkretnej komendy
dotnet run -- download-file --help
```

### Przykład: download-file

```bash
# Podstawowe użycie
dotnet run -- download-file test123

# Z opcją --full
dotnet run -- download-file test123 --full

# Zapis do pliku
dotnet run -- download-file test123 --output "./myfile.txt"

# Verbose output
dotnet run -- download-file test123 --full --verbose

# Wszystkie opcje razem
dotnet run -- download-file test123 --full --output "./myfile.txt" --verbose
```

## Jak działa autentykacja

1. Przy pierwszym wywołaniu komendy, aplikacja automatycznie loguje się do API
2. Otrzymuje token autoryzacyjny z odpowiedzi logowania
3. Token jest cachowany i używany we wszystkich kolejnych requestach
4. Format odpowiedzi logowania można dostosować w `Services/AuthenticationService.cs`

### Format odpowiedzi API logowania

Aplikacja obsługuje następujące formaty odpowiedzi:

```json
// Format 1
{ "token": "your-token-here" }

// Format 2
{ "accessToken": "your-token-here" }

// Format 3
{ "data": { "token": "your-token-here" } }
```

Jeśli Twój API używa innego formatu, edytuj metodę `GetAuthenticationTokenAsync()` w `Services/AuthenticationService.cs`.

## Rozbudowa o nowe komendy

### 1. Utwórz klasę komendy

Stwórz nowy plik w folderze `Commands/`, np. `UploadFileCommand.cs`:

```csharp
using RestApiClient.Services;

namespace RestApiClient.Commands;

public class UploadFileCommand : BaseCommand
{
    private readonly string _filePath;

    public UploadFileCommand(
        ApiClient apiClient,
        AuthenticationService authService,
        bool verbose,
        string filePath)
        : base(apiClient, authService, verbose)
    {
        _filePath = filePath;
    }

    public override async Task<int> ExecuteAsync()
    {
        try
        {
            // Authenticate
            var token = await AuthService.GetAuthenticationTokenAsync();

            // Read file
            var fileContent = await File.ReadAllTextAsync(_filePath);

            // Send request
            var response = await ApiClient.SendRequestAsync(
                method: "POST",
                endpoint: "files/upload",
                body: fileContent,
                authToken: token
            );

            Console.WriteLine(response);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}
```

### 2. Zarejestruj komendę w Program.cs

Dodaj do `Program.cs`:

```csharp
// upload-file command
var uploadFileCommand = new Command("upload-file", "Upload a file to the API");

var filePathArgument = new Argument<string>(
    "file-path",
    "Path to file to upload"
);
uploadFileCommand.AddArgument(filePathArgument);

uploadFileCommand.SetHandler(async (InvocationContext ctx) =>
{
    var verbose = ctx.ParseResult.GetValueForOption(verboseOption);
    var filePath = ctx.ParseResult.GetValueForArgument(filePathArgument);

    // Update API client verbosity
    apiClient = new ApiClient(
        baseAddress: apiSettings.BaseAddress,
        timeoutSeconds: apiSettings.TimeoutSeconds,
        authSettings: authSettings,
        verbose: verbose
    );
    
    authService = new AuthenticationService(configService, apiClient);

    var command = new UploadFileCommand(
        apiClient,
        authService,
        verbose,
        filePath
    );

    var exitCode = await command.ExecuteAsync();
    Environment.Exit(exitCode);
});

rootCommand.Add(uploadFileCommand);
```

### 3. Gotowe!

Teraz możesz używać nowej komendy:

```bash
dotnet run -- upload-file ./myfile.txt
```

## Struktura projektu

```
RestApiTest/
├── appsettings.json              # Konfiguracja (base address, login, hasło)
├── Program.cs                    # Główny program z rejestracją komend
├── Models/
│   └── ApiSettings.cs           # Modele konfiguracji
├── Services/
│   ├── ApiClient.cs             # Klient HTTP z obsługą tokenu
│   ├── AuthenticationService.cs # Serwis autentykacji
│   └── ConfigurationService.cs  # Serwis konfiguracji
└── Commands/
    ├── BaseCommand.cs           # Bazowa klasa dla komend
    └── DownloadFileCommand.cs   # Przykładowa komenda
```

## Dostosowanie endpointów

Endpointy są hardkodowane w klasach komend. Aby zmienić endpoint, edytuj odpowiednią klasę komendy:

```csharp
// W DownloadFileCommand.cs
string endpoint = _full 
    ? $"files/{{fileId}}/full"    // files/123/full
    : $"files/{{fileId}}";         // files/123
```

Parametry w endpointach są przekazywane przez słownik:

```csharp
var parameters = new Dictionary<string, string>
{
    ["fileId"] = _fileId
};
```

## Budowanie i uruchamianie

```bash
# Development
dotnet run -- download-file test123

# Build
dotnet build

# Publish
dotnet publish -c Release

# Uruchomienie skompilowanej wersji
./bin/Release/net8.0/RestApiClient download-file test123
```

## Uwagi

- Token autoryzacyjny jest cachowany na czas życia aplikacji
- Base address jest konfigurowany tylko przez `appsettings.json`
- Wszystkie komendy automatycznie logują się przed wykonaniem requestu
- Aby dodać więcej opcji do komendy, użyj `Option<T>` w System.CommandLine

## Licencja

MIT