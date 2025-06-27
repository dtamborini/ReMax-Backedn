using UserService.Models.DTOs;

namespace UserService.Models.DTOs
{
    public interface IUserDataProviderClient
    {
        Task<IEnumerable<UserDto>?> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto?> CreateUserAsync(UserDto newBuilding);
        Task<bool> UpdateUserAsync(Guid id, UserDto updatedBuilding);
        Task<bool> DeleteUserAsync(Guid id);
    }
}