using BuildingService.Clients;
using BuildingService.Models;
using System.Net.Http.Headers;

public class BuildingDataProviderClient : IBuildingDataProviderClient
{
    private readonly HttpClient _httpClient;
    private readonly string _buildingsApiEndpoint;
    private readonly string? _jsioApiToken;

    public BuildingDataProviderClient(HttpClient httpClient, IConfiguration configuration)
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
        var request = CreateJsonRequest(HttpMethod.Get, _buildingsApiEndpoint);
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<BuildingDto>>();
    }

    public async Task<BuildingDto?> GetBuildingByIdAsync(Guid id)
    {
        var request = CreateJsonRequest(HttpMethod.Get, $"{_buildingsApiEndpoint}/{id}");
        var response = await _httpClient.GetAsync($"{_buildingsApiEndpoint}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BuildingDto>();
        }
        return null;
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