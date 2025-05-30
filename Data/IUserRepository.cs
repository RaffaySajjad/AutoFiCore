using AutoFiCore.Models;

namespace AutoFiCore.Data
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User?> LoginUserAsync(string email, string password);

    }
}
