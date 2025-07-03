using BuildingService.Clients;
using BuildingService.Models;
using BuildingService.Converters;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonServerBuildingDataProviderClient : IBuildingDataProviderClient
{
    private readonly HttpClient _httpClient;
    private readonly string _buildingsApiEndpoint;
    private readonly string? _jsioApiToken;

    public JsonServerBuildingDataProviderClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _buildingsApiEndpoint = configuration["BuildingDataProviderService:BuildingsApiEndpoint"] ?? "/api/buildings";

        _jsioApiToken = configuration["BuildingDataProviderService:ApiToken"];
    }

    private HttpRequestMessage CreateJsonRequest(HttpMethod method, string requestUri, object? content = null)
    {
        var request = new HttpRequestMessage(method, requestUri);
        if (!string.IsNullOrEmpty(_jsioApiToken))
        {
            request.Headers.Add("X-Jsio-Token", _jsioApiToken);
        }

        if (content != null)
        {
            request.Content = JsonContent.Create(content);
        }

        return request;
    }

    public async Task<IEnumerable<BuildingDto>?> GetBuildingsAsync()
    {
        string baseDirectory = AppContext.BaseDirectory;
        string jsonFilePath = Path.Combine(baseDirectory, "Resources", "schema_buildings.json");
        string jsonContent = await File.ReadAllTextAsync(jsonFilePath);


        var request = CreateJsonRequest(HttpMethod.Post, _buildingsApiEndpoint);
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            Converters = {
                new JsonStringEnumConverter(),
                new CustomUtcDateTimeConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        try
        {

            return await response.Content.ReadFromJsonAsync<IEnumerable<BuildingDto>>(options);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Errore di deserializzazione JSON: {ex.Message}");
            return null;
        }
    }

    public async Task<BuildingDto?> GetBuildingByIdAsync(Guid id)
    {
        string baseDirectory = AppContext.BaseDirectory;
        string jsonFilePath = Path.Combine(baseDirectory, "Resources", "schema_building.json");
        string jsonContent = await File.ReadAllTextAsync(jsonFilePath);

        var request = CreateJsonRequest(HttpMethod.Post, $"{_buildingsApiEndpoint}/{id}");
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            Converters = { 
                new JsonStringEnumConverter(),
                new CustomUtcDateTimeConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        try
        {

            return await response.Content.ReadFromJsonAsync<BuildingDto>(options);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Errore di deserializzazione JSON: {ex.Message}");
            return null;
        }
    }

    public async Task<BuildingDto?> CreateBuildingAsync(BuildingDto newBuilding)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _buildingsApiEndpoint);

        var response = await _httpClient.PostAsJsonAsync(_buildingsApiEndpoint, newBuilding);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BuildingDto>();
    }

    public async Task<bool> UpdateBuildingAsync(Guid id, BuildingDto updatedBuilding)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _buildingsApiEndpoint);

        var response = await _httpClient.PutAsJsonAsync($"{_buildingsApiEndpoint}/{id}", updatedBuilding);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteBuildingAsync(Guid id)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _buildingsApiEndpoint);

        var response = await _httpClient.DeleteAsync($"{_buildingsApiEndpoint}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }
}