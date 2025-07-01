using BuildingService.Models.MappingDao;
using BuildingService.Enums;

namespace BuildingService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<EntityMapping> GetMappingByGuidAsync(Guid mappingGuid);

        Task<IEnumerable<EntityMapping>> GetMappingsWithOptionalParameters(
            bool? isActive = null,
            EntityType? entityType = null);
    }
}