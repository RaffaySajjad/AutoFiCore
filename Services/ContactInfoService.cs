using AutoFiCore.Data;
using AutoFiCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoFiCore.Services
{
    public interface IContactInfoService
    {
        Task<ContactInfo> AddContactInfoAsync(ContactInfo contactInfo);

    }

    public class ContactInfoService : IContactInfoService
    {
        private readonly ApplicationDbContext _dbContext;

        public ContactInfoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ContactInfo> AddContactInfoAsync(ContactInfo contactInfo)
        {
            _dbContext.ContactInfos.Add(contactInfo);
            await _dbContext.SaveChangesAsync();
            return contactInfo;
        }
    }
}
