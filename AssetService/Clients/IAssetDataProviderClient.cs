using AssetService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetService.Clients
{
    public interface IAssetDataProviderClient
    {
        Task<IEnumerable<AssetDto>?> GetAssetAsync();
        Task<AssetDto?> GetAssetByIdAsync(Guid id);
        Task<AssetDto?> CreateAssetAsync(AssetDto newBuilding);
        Task<bool> UpdateAssetAsync(Guid id, AssetDto updatedBuilding);
        Task<bool> DeleteAssetAsync(Guid id);
    }
}