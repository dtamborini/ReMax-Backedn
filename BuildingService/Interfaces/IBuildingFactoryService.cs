using BuildingService.Models.MappingDao;
using BuildingService.Models;

namespace BuildingService.Interfaces
{
    public interface IBuildingFactoryService
    {
        Building MapBuildingDto(
            BuildingDto buildingDto,
            EntityMapping entityMapping,
            Guid userUuid
        );
    }
}