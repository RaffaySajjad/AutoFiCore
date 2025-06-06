using AutoFiCore.Dto;
using AutoFiCore.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace AutoFiCore.Data
{
    public class DbUserRepository:IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DbUserRepository> _logger;
        public DbUserRepository(ApplicationDbContext dbContext, ILogger<DbUserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<bool> IsEmailExists(string email)
        {
            try
            {
                var isExists = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Email == email);
                if (isExists)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email");
                throw;
            }
        }
        public async Task<User?> AddUserAsync(User user)
        {
            try
            {
                if (await IsEmailExists(user.Email))
                {
                    return null;
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new user");
                throw;

            }
        }
        public async Task<UserSavedSearch> AddUserSearchAsync(UserSavedSearch search)
        {
            try
            {
                _dbContext.UserSavedSearches.Add(search);
                await _dbContext.SaveChangesAsync();
                return search;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user search");
                throw;

            }
        }
        public async Task<UserSavedSearch?> RemoveUserSearchAsync(UserSavedSearch search)
        {
            try
            {
                var savedSearch = await _dbContext.UserSavedSearches.FirstOrDefaultAsync(us => us.userId == search.userId && us.search == search.search);
                if (savedSearch == null)
                {
                    return null;
                }
                _dbContext.UserSavedSearches.Remove(savedSearch);
                await _dbContext.SaveChangesAsync();
                return search;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing saved search");
                throw;

            }
        }
        public async Task<List<string>> GetUserLikesVehicles(int id)
        {
            try
            {
                return await _dbContext.UserLikes
                    .AsNoTracking()
                    .Where(ul => ul.userId == id)
                    .Select(ul => ul.vehicleVin)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user liked vins");
                throw;
            }
        }

        public async Task<List<string>> GetUserSavedSearches(int id)
        {
            try
            {
                return await _dbContext.UserSavedSearches
                    .AsNoTracking()
                    .Where(us => us.userId == id)
                    .Select(us => us.search)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user saved searches");
                throw;
            }
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new user");
                throw;

            }
        }
        public async Task<UserLikes> AddUserLikeAsync(UserLikes userlikes)
        {
            try
            {
                _dbContext.UserLikes.Add(userlikes);
                await _dbContext.SaveChangesAsync();
                return userlikes;
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
                var like = await _dbContext.UserLikes
                    .FirstOrDefaultAsync(ul => ul.userId == userLikes.userId && ul.vehicleVin == userLikes.vehicleVin);

                if (like == null)
                {
                    return null;
                }

                _dbContext.UserLikes.Remove(like);
                await _dbContext.SaveChangesAsync();
                return like;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user like");
                throw;
            }
        }
        public async Task<AuthResponse?> LoginUserAsync(string email, string password, TokenProvider tokenProvider)
        {
            try
            {

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return null;  
                }
                if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return null;
                }

                string token = tokenProvider.Create(user);
                return new AuthResponse
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.Name,
                    UserEmail = user.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user like");
                throw;
            }
        }
    }


 }

