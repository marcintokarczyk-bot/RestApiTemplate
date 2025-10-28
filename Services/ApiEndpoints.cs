namespace RestApiClient.Services;

public static class ApiEndpoints
{
    public static Dictionary<string, string> GetEndpoints()
    {
        return new Dictionary<string, string>
        {
            // Test endpoints (httpbin.org)
            ["get"] = "get",
            ["get-params"] = "anything/{anythingId}",
            ["post"] = "post",
            ["put"] = "put",
            ["delete"] = "delete",
            
            // Files endpoints
            ["files-get"] = "files/{fileId}",
            ["files-full"] = "files/{fileId}/full",
            ["files-list"] = "files",
            
            // Users endpoints
            ["users-get"] = "users/{userId}",
            ["users-list"] = "users",
            ["users-create"] = "users",
            
            // Documents endpoints
            ["docs-get"] = "documents/{docId}",
            ["docs-update"] = "documents/{docId}",
            ["docs-list"] = "documents",
            
            // Examples with multiple parameters
            ["files-download"] = "files/{fileId}/download/{version}",
            ["users-files"] = "users/{userId}/files/{fileId}"
        };
    }

    public static void PrintAvailableEndpoints()
    {
        Console.WriteLine("\nDostępne endpointy:");
        Console.WriteLine("==================\n");
        
        var endpoints = GetEndpoints();
        
        foreach (var endpoint in endpoints)
        {
            Console.WriteLine($"  {endpoint.Key.PadRight(20)} -> {endpoint.Value}");
        }
        
        Console.WriteLine();
    }

    public static bool IsEndpointValid(string endpointName)
    {
        return GetEndpoints().ContainsKey(endpointName);
    }

    public static string GetEndpointPath(string endpointName)
    {
        if (!GetEndpoints().TryGetValue(endpointName, out var path))
        {
            throw new ArgumentException($"Endpoint '{endpointName}' not found");
        }
        return path;
    }
}
