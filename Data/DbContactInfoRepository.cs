using AutoFiCore.Models;
using AutoFiCore.Services;
using AutoFiCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AutoFiCore.Data
{
    public class DbContactInfoRepository:IContactInfoRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DbContactInfoRepository> _logger;
        public DbContactInfoRepository(ApplicationDbContext dbContext, ILogger<DbContactInfoRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<ContactInfo> AddContactInfoAsync(ContactInfo contactInfo)
        {
           try
            {
                _dbContext.ContactInfos.Add(contactInfo);
                await _dbContext.SaveChangesAsync();
                return contactInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding contact info");
                throw;

            }
        }
    }
}

