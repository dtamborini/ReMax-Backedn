using System.Net.Http;
using System.Text.Json;

namespace QuoteService.Clients
{
    public class MappingServiceHttpClient : IMappingServiceHttpClient
    {
        private readonly HttpClient _httpClient;

        public MappingServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Guid?> GetMappingGuidByIdAsync(Guid mappingId)
        {

            var response = await _httpClient.GetAsync($"/api/mappings/{mappingId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var mappingResponse = JsonSerializer.Deserialize<MappingResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return mappingResponse?.Guid;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing mapping response: {ex.Message}");
                    return null;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Error parsing GUID from mapping response: {ex.Message}");
                    return null;
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Mapping with ID {mappingId} not found.");
                return null;
            }
            else
            {
                Console.WriteLine($"Error calling MappingService: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }
    }
}

public class MappingResponseDto
{
    public Guid Guid { get; set; }
}