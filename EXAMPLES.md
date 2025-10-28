# Przykłady użycia REST API Client

## Podstawowe użycie

### Lista dostępnych endpointów

```bash
dotnet run -- --list
```

### Prosty GET request

```bash
dotnet run -- -b https://httpbin.org get
```

### GET z verbose output

```bash
dotnet run -- -b https://httpbin.org -v get
```

## Endpointy z parametrami

### Pojedynczy parametr

```bash
dotnet run -- -b https://httpbin.org get-params -p "anythingId=test123"
```

Wynik: Wyśle request do `https://httpbin.org/anything/test123`

### Wiele parametrów (przykład endpointu z 2 parametrami)

```bash
dotnet run -- -b https://api.example.com files-download -p "fileId=123" -p "version=2"
```

Wynik: Wyśle request do `https://api.example.com/files/123/download/2`

## POST request z body

```bash
dotnet run -- -b https://httpbin.org -m POST -d '{"key":"value"}' post
```

## Headers

```bash
dotnet run -- -b https://httpbin.org get -H "Authorization:Bearer token123" -H "Custom-Header:Value"
```

## Konfiguracja timeout

```bash
dotnet run -- -b https://httpbin.org -t 60 get
```

## Zapis odpowiedzi do pliku

```bash
dotnet run -- -b https://httpbin.org get -o response.json
```

## Zaawansowane przykłady

### Z wszystkimi opcjami

```bash
dotnet run -- \
  -b https://httpbin.org \
  -m POST \
  -v \
  -H "Authorization:Bearer token123" \
  -H "Content-Type:application/json" \
  -d '{"test":"data"}' \
  -t 30 \
  -o output.json \
  post
```

### Kombinacja verbose + parametrów

```bash
dotnet run -- -b https://httpbin.org -v get-params -p "anythingId=test"
```

Output będzie zawierał:
- Pełny URL (`https://httpbin.org/anything/test`)
- Wszystkie nagłówki requestu
- Pełną odpowiedź API

## Rzeczywiste endpointy (do skonfigurowania w ApiEndpoints.cs)

Przykłady dostępne po skonfigurowaniu:

```bash
# Pobranie pliku
dotnet run -- -b https://api.example.com files-get -p "fileId=12345"

# Pobranie pełnej informacji o pliku
dotnet run -- -b https://api.example.com files-full -p "fileId=12345"

# Lista użytkowników
dotnet run -- -b https://api.example.com users-list

# Utworzenie użytkownika
dotnet run -- -b https://api.example.com -m POST users-create -d '{"name":"Test"}'

# Pobranie użytkownika
dotnet run -- -b https://api.example.com users-get -p "userId=123"
```

