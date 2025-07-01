using WorkSheetService.Models.MappingDao;
using WorkSheetService.Enums;

namespace WorkSheetService.Clients
{
    public interface IMappingServiceHttpClient
    {
        Task<EntityMapping> GetMappingByGuidAsync(Guid mappingGuid);

        Task<IEnumerable<EntityMapping>> GetMappingsWithOptionalParameters(
            bool? isActive = null,
            EntityType? entityType = null);
    }
}