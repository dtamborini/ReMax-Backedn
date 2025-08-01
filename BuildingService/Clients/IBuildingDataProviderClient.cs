using BuildingService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingService.Clients
{
    public interface IBuildingDataProviderClient
    {
        Task<IEnumerable<BuildingDto>?> GetBuildingsAsync();
        Task<BuildingDto?> GetBuildingByIdAsync(Guid id);
        Task<BuildingDto?> CreateBuildingAsync(BuildingDto newBuilding);
        Task<bool> UpdateBuildingAsync(Guid id, BuildingDto updatedBuilding);
        Task<bool> DeleteBuildingAsync(Guid id);
    }

    public interface IUserFilteredBuildingDataProviderClient : IBuildingDataProviderClient
    {
        Task<IEnumerable<BuildingDto>?> GetBuildingsForUserAsync(string userId);
    }
}