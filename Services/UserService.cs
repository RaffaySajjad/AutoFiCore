using AutoFiCore.Data;
using AutoFiCore.Dto;
using AutoFiCore.Models;

namespace AutoFiCore.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
        Task<AuthResponse> LoginUserAsync(string email, string password);
    }

    public class UserService:IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UserService> _logger;
        private readonly TokenProvider _tokenProvider;
        public UserService(IUserRepository repository, ILogger<UserService> logger, TokenProvider tokenProvider)
        {
            _repository = repository;
            _logger = logger;
            _tokenProvider = tokenProvider;
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
        public async Task<AuthResponse> LoginUserAsync(string email, string password)
        {
            try
            {
                return await _repository.LoginUserAsync(email, password, _tokenProvider);

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                throw;
            }
        }

    }
}
