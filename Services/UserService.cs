using AutoFiCore.Data;
using AutoFiCore.Models;

namespace AutoFiCore.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
    }

    public class UserService:IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository repository, ILogger<UserService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                return await _repository.AddUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new user");
                throw;

            }
        }
    }
}
