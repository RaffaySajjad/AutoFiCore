using AutoFiCore.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

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
        public async Task<User> AddUserAsync(User user)
        {
            try
            {
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
        public async Task<User?> LoginUserAsync(string email, string password)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                throw;
            }
        }

    }
}
