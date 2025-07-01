using RfqService.Models.MappingDao;
using RfqService.Enums;

namespace RfqService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<EntityMapping> GetMappingByGuidAsync(Guid mappingGuid);

        Task<IEnumerable<EntityMapping>> GetMappingsWithOptionalParameters(
            bool? isActive = null,
            EntityType? entityType = null);
    }
}