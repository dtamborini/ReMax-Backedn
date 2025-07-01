using UserService.Models.MappingDao;
using UserService.Enums;

namespace UserService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<EntityMapping> GetMappingByGuidAsync(Guid mappingGuid);

        Task<IEnumerable<EntityMapping>> GetMappingsWithOptionalParameters(
            bool? isActive = null,
            EntityType? entityType = null);
    }
}