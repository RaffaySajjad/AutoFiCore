using AutoFiCore.Data;
using AutoFiCore.Dto;
using AutoFiCore.Models;

namespace AutoFiCore.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
        Task<AuthResponse?> LoginUserAsync(string email, string password);
        Task<UserLikes> AddUserLikeAsync(UserLikes userlikes);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<string>> GetUserLikedVinsAsync(int id);
        Task<UserLikes?> RemoveUserLikeAsync(UserLikes userLikes);
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
        public async Task<AuthResponse?> LoginUserAsync(string email, string password)
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
        public async Task<UserLikes> AddUserLikeAsync(UserLikes userLikes)
        {
            try
            {

                return await _repository.AddUserLikeAsync(userLikes);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user like");
                throw;
            }
        }
        public async Task<UserLikes?> RemoveUserLikeAsync(UserLikes userLikes)
        {
            try
            {
                return await _repository.RemoveUserLikeAsync(userLikes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user like");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _repository.GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {Id}", id);
                throw;
            }
        }

        public async Task<List<string>> GetUserLikedVinsAsync (int id)
        {
            try
            {
                return await _repository.GetUserLikesVehicles(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user liked vins");
                throw;
            }
        }
    }
}
