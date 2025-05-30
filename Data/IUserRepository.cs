using AutoFiCore.Dto;
using AutoFiCore.Models;

namespace AutoFiCore.Data
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<AuthResponse> LoginUserAsync(string email, string password, TokenProvider tokenProvider);
    }
}
