using QuoteService.Models.MappingDao;
using QuoteService.Enums;

namespace QuoteService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<EntityMapping> GetMappingByGuidAsync(Guid mappingGuid);

        Task<IEnumerable<EntityMapping>> GetMappingsWithOptionalParameters(
            bool? isActive = null,
            EntityType? entityType = null);
    }
}