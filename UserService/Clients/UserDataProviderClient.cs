using UserService.Models.DTOs;
using UserService.Models;
using System.Net.Http.Headers;

public class UserDataProviderClient : IUserDataProviderClient
{
    private readonly HttpClient _httpClient;
    private readonly string _userApiEndpoint;
    private readonly string? _jsioApiToken;

    public UserDataProviderClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _userApiEndpoint = configuration["UserDataProviderService:UserApiEndpoint"] ?? "/api/users";

        _jsioApiToken = configuration["UserDataProviderService:ApiToken"];
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

    public async Task<IEnumerable<UserDto>?> GetUsersAsync()
    {
        var request = CreateJsonRequest(HttpMethod.Get, _userApiEndpoint);
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var request = CreateJsonRequest(HttpMethod.Get, $"{_userApiEndpoint}/{id}");
        var response = await _httpClient.GetAsync($"{_userApiEndpoint}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
        return null;
    }

    public async Task<UserDto?> CreateUserAsync(UserDto newBuilding)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _userApiEndpoint);

        var response = await _httpClient.PostAsJsonAsync(_userApiEndpoint, newBuilding);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserDto updatedBuilding)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _userApiEndpoint);

        var response = await _httpClient.PutAsJsonAsync($"{_userApiEndpoint}/{id}", updatedBuilding);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var request = CreateJsonRequest(HttpMethod.Get, _userApiEndpoint);

        var response = await _httpClient.DeleteAsync($"{_userApiEndpoint}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }
}