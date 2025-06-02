using AutoFiCore.Dto;
using AutoFiCore.Models;

namespace AutoFiCore.Data
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<AuthResponse?> LoginUserAsync(string email, string password, TokenProvider tokenProvider);
        Task<UserLikes> AddUserLikeAsync(UserLikes userlikes);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<string>> GetUserLikesVehicles(int id);
        Task<UserLikes?> RemoveUserLikeAsync(UserLikes userLikes);
    }
}
