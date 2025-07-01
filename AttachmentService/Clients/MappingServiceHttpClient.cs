using AttachmentService.Enums;
using AttachmentService.Models.MappingDao;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AttachmentService.Clients
{
    public class MappingServiceHttpClient : IMappingServiceHttpClient
    {
        private readonly HttpClient _httpClient;

        public MappingServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EntityMapping> GetMappingByGuidAsync(Guid mappingGuid)
        {
            var response = await _httpClient.GetAsync($"/api/mappings/{mappingGuid}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var mapping = await response.Content.ReadFromJsonAsync<EntityMapping>(
                        new JsonSerializerOptions { 
                            PropertyNameCaseInsensitive = true,
                            Converters = { new JsonStringEnumConverter() }
                        });

                    if (mapping == null)
                    {
                        throw new InvalidOperationException(
                            $"La risposta per il mapping con ID {mappingGuid} era vuota o non valida, ma lo stato HTTP era {response.StatusCode}.");
                    }

                    return mapping;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException(
                        $"Errore di deserializzazione dalla risposta del mapping per ID {mappingGuid}: {ex.Message}", ex);
                }
                catch (NotSupportedException ex)
                {
                    throw new InvalidOperationException(
                        $"Tipo di contenuto non supportato per la deserializzazione per ID {mappingGuid}: {ex.Message}", ex);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"Mapping con ID {mappingGuid} non trovato.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Errore nella chiamata al MappingService per ID {mappingGuid}: {response.StatusCode} - {errorContent}");
            }
        }

        public async Task<IEnumerable<EntityMapping>> GetMappingsWithOptionalParameters(
            bool? isActive = null,
            EntityType? entityType = null)
        {

            var url = $"/api/mappings";

            var queryParameters = new List<string>();

            if (isActive.HasValue)
            {
                queryParameters.Add($"isActive={isActive.Value}");
            }

            if (entityType.HasValue)
            {
                queryParameters.Add($"entityType={entityType.Value}");
            }

            if (queryParameters.Any())
            {
                url += "?" + string.Join("&", queryParameters);
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var mappings = await response.Content.ReadFromJsonAsync<IEnumerable<EntityMapping>>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (mappings == null || !mappings.Any())
            {
                throw new InvalidOperationException("Nessun mapping trovato per i criteri specificati o la lista è vuota.");
            }
            return mappings;
        }
    }
}