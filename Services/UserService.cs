using AutoFiCore.Data;
using AutoFiCore.Dto;
using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFiCore.Services
{
    public interface IUserService
    {
        Task<User?> AddUserAsync(User user);
        Task<AuthResponse?> LoginUserAsync(string email, string password);
        Task<UserLikes> AddUserLikeAsync(UserLikes userlikes);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<string>> GetUserLikedVinsAsync(int id);
        Task<UserLikes?> RemoveUserLikeAsync(UserLikes userLikes);
        Task<UserSavedSearch> AddUserSearchAsync(UserSavedSearch search);
        Task<UserSavedSearch?> RemoveSavedSearchAsync(UserSavedSearch savedSearch);
        Task<List<string>> GetUserSavedSearches(int id);
        Task<UserInteractions> AddUserInteractionAsync(UserInteractions userInteractions);
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

        public async Task<User?> AddUserAsync(User user)
        {
            return await _repository.AddUserAsync(user);
        }
        public async Task<AuthResponse?> LoginUserAsync(string email, string password)
        {
            return await _repository.LoginUserAsync(email, password, _tokenProvider);
        }
        public async Task<UserLikes> AddUserLikeAsync(UserLikes userLikes)
        {
            return await _repository.AddUserLikeAsync(userLikes);
        }
        public async Task<UserLikes?> RemoveUserLikeAsync(UserLikes userLikes)
        {
            return await _repository.RemoveUserLikeAsync(userLikes);
        }
        public async Task<UserSavedSearch?> RemoveSavedSearchAsync(UserSavedSearch savedSearch)
        {
            return await _repository.RemoveUserSearchAsync(savedSearch);
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repository.GetUserByIdAsync(id);
        }
        public async Task<UserSavedSearch> AddUserSearchAsync(UserSavedSearch search)
        {
            return await _repository.AddUserSearchAsync(search);
        }
        public async Task<List<string>> GetUserLikedVinsAsync (int id)
        {
            return await _repository.GetUserLikesVehicles(id);
        }
        public async Task<List<string>> GetUserSavedSearches(int id)
        {
            return await _repository.GetUserSavedSearches(id);
        }
        public async Task<UserInteractions> AddUserInteractionAsync(UserInteractions userInteractions)
        {
            return await _repository.AddUserInteraction(userInteractions);
        }

    }
}
