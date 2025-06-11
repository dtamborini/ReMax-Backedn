using System.Net.Http;
using System.Text.Json;

namespace UserService.Clients
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
            // Assumiamo che il MappingService abbia un endpoint come GET /api/mappings/{id}
            // e che restituisca un JSON contenente il GUID, o direttamente il GUID.
            // Dovrai adattare questo in base a come il tuo MappingService è progettato.

            var response = await _httpClient.GetAsync($"/api/mappings/{mappingId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Assumiamo che la risposta sia un oggetto JSON con un campo "id" o "mappingId"
                // che contiene il GUID. Se restituisce solo il GUID come stringa, la deserializzazione è più semplice.
                try
                {
                    // Scenario 1: La risposta è direttamente il GUID come stringa
                    // return Guid.Parse(content);

                    // Scenario 2: La risposta è un oggetto JSON, es. { "id": "...", "name": "..." }
                    var mappingResponse = JsonSerializer.Deserialize<MappingResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return mappingResponse?.Id;
                }
                catch (JsonException ex)
                {
                    // Gestisci l'errore di deserializzazione
                    Console.WriteLine($"Error deserializing mapping response: {ex.Message}");
                    return null;
                }
                catch (FormatException ex)
                {
                    // Gestisci l'errore di parsing del GUID
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
                // Logga l'errore o lancia un'eccezione
                Console.WriteLine($"Error calling MappingService: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }
    }
}

public class MappingResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // ... altre proprietà
}