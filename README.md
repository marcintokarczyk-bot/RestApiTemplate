# REST API Client CLI

CLI narzędzie w C# do wysyłania zapytań HTTP/REST API z konfigurowalnym base address i endpointami.

## Funkcje

- ✅ Base Address dla wszystkich żądań
- ✅ Predefiniowane endpointy z parametrami (np. "files/{fileId}")
- ✅ Wysyłanie zapytań GET, POST, PUT, DELETE, PATCH
- ✅ Dodawanie niestandardowych nagłówków
- ✅ Obsługa body dla żądań POST/PUT/PATCH
- ✅ Verbose output z pełnym URL
- ✅ Zapisywanie odpowiedzi do pliku
- ✅ Konfigurowalny timeout
- ✅ Lista dostępnych endpointów

## Instalacja

```bash
# Zainstaluj .NET SDK 8.0 jeśli nie masz
# Następnie uruchom:
dotnet restore
dotnet build
```

## Użycie

### Lista dostępnych endpointów

```bash
dotnet run -- --list
```

### Podstawowe zapytanie GET

```bash
dotnet run -- -b https://api.example.com files-list
```

### Endpoint z parametrem

```bash
dotnet run -- -b https://api.example.com files-get -p fileId=123
```

### Wiele parametrów

```bash
dotnet run -- -b https://api.example.com files-download -p "fileId=123" -p "version=2"
```

### Zapytanie POST z body

```bash
dotnet run -- -b https://api.example.com -m POST -d '{"name":"test"}' users-create
```

### Dodawanie nagłówków

```bash
dotnet run -- -b https://api.example.com -H "Authorization:Bearer token123" files-get -p fileId=123
```

### Verbose output (pokazuje pełny URL)

```bash
dotnet run -- -b https://api.example.com -v files-get -p fileId=123
```

### Zapisywanie odpowiedzi do pliku

```bash
dotnet run -- -b https://api.example.com files-get -p fileId=123 -o response.json
```

### Konfiguracja timeout

```bash
dotnet run -- -b https://api.example.com files-get -t 60 -p fileId=123
```

## Dostępne Endpointy

Możesz zobaczyć wszystkie dostępne endpointy używając:

```bash
dotnet run -- --list
```

Przykładowe endpointy:
- `files-get` → `files/{fileId}`
- `files-full` → `files/{fileId}/full`
- `files-list` → `files`
- `users-get` → `users/{userId}`
- `users-list` → `users`
- `files-download` → `files/{fileId}/download/{version}` (wymaga fileId i version)

## Opcje

| Opcja | Skrót | Opis | Wymagany |
|-------|-------|------|----------|
| `--base` | `-b` | Base address API | **TAK** |
| `--method` | `-m` | Metoda HTTP (GET, POST, PUT, DELETE, PATCH) | Nie (domyślnie: GET) |
| `--param` | `-p` | Parametry endpointu (format: key=value) | Nie |
| `--header` | `-H` | Nagłówki (format: Key:Value) | Nie |
| `--body` | `--data`, `-d` | Body dla POST/PUT/PATCH | Nie |
| `--timeout` | `-t` | Timeout w sekundach | Nie (domyślnie: 30) |
| `--output` | `-o` | Ścieżka pliku wyjściowego | Nie |
| `--verbose` | `-v` | Verbose output | Nie |
| `--list` | `-l` | Lista dostępnych endpointów | Nie |

## Przykłady

### Pobranie listy plików

```bash
dotnet run -- -b https://api.example.com files-list
```

### Pobranie konkretnego pliku

```bash
dotnet run -- -b https://api.example.com files-get -p fileId=12345
```

### Pobranie pełnej informacji o pliku

```bash
dotnet run -- -b https://api.example.com files-full -p fileId=12345
```

### Utworzenie nowego użytkownika

```bash
dotnet run -- -b https://api.example.com -m POST \
  -H "Content-Type:application/json" \
  -H "Authorization:Bearer YOUR_TOKEN" \
  -d '{"name":"John Doe","email":"john@example.com"}' \
  users-create
```

### Pobranie pliku z określoną wersją

```bash
dotnet run -- -b https://api.example.com \
  files-download \
  -p fileId=123 \
  -p version=2
```

### Aktualizacja dokumentu (PUT)

```bash
dotnet run -- -b https://api.example.com -m PUT \
  -d '{"title":"Updated Title"}' \
  docs-update \
  -p docId=456
```

### Usuwanie zasobu (DELETE)

```bash
dotnet run -- -b https://api.example.com -m DELETE \
  files-get \
  -p fileId=123
```

## Dodawanie własnych endpointów

Aby dodać własne endpointy, edytuj plik `Services/ApiEndpoints.cs` i dodaj nowe wpisy do słownika:

```csharp
public static Dictionary<string, string> GetEndpoints()
{
    return new Dictionary<string, string>
    {
        // ... istniejące endpointy ...
        
        // Twój nowy endpoint
        ["custom-endpoint"] = "custom/path/{param1}/{param2}",
    };
}
```

## Budowanie i uruchamianie

```bash
# Development
dotnet run -- -b https://api.example.com [endpoint] [opcje]

# Build
dotnet build

# Publish
dotnet publish -c Release

# Uruchomienie skompilowanej wersji
./bin/Release/net8.0/RestApiClient -b https://api.example.com [endpoint] [opcje]
```

## Kod źródłowy

- `Program.cs` - Główny program z parsowaniem argumentów CLI
- `Services/ApiClient.cs` - Serwis do obsługi zapytań HTTP
- `Services/ApiEndpoints.cs` - Definicje endpointów

## Licencja

MIT