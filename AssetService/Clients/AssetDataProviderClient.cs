using AssetService.Clients;
using AssetService.Models;
using System.Net.Http.Headers;

public class AssetDataProviderClient : IAssetDataProviderClient
{
    private readonly HttpClient _httpClient;
    private readonly string _assetsApiEndpoint;
    private readonly string? _jsioApiToken;

    public AssetDataProviderClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _assetsApiEndpoint = configuration["AssetDataProviderService:AssetsApiEndpoint"] ?? "/api/assets";

        _jsioApiToken = configuration["AssetDataProviderService:ApiToken"];
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

    public async Task<IEnumerable<AssetDto>?> GetAssetAsync()
    {
        var request = CreateJsonRequest(HttpMethod.Get, _assetsApiEndpoint);
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<AssetDto>>();
    }

    public async Task<AssetDto?> GetAssetByIdAsync(Guid id)
    {
        var request = CreateJsonRequest(HttpMethod.Get, $"{_assetsApiEndpoint}/{id}");
        var response = await _httpClient.GetAsync($"{_assetsApiEndpoint}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AssetDto>();
        }
        return null;
    }

    public async Task<AssetDto?> CreateAssetAsync(AssetDto newAsset)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _assetsApiEndpoint);

        var response = await _httpClient.PostAsJsonAsync(_assetsApiEndpoint, newAsset);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AssetDto>();
    }

    public async Task<bool> UpdateAssetAsync(Guid id, AssetDto updatedAsset)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _assetsApiEndpoint);

        var response = await _httpClient.PutAsJsonAsync($"{_assetsApiEndpoint}/{id}", updatedAsset);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteAssetAsync(Guid id)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _assetsApiEndpoint);

        var response = await _httpClient.DeleteAsync($"{_assetsApiEndpoint}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }
}