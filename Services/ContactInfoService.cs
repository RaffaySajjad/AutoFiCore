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
        private readonly IContactInfoRepository _repository;
        private readonly ILogger<ContactInfoService> _logger;
        public ContactInfoService(IContactInfoRepository repository, ILogger<ContactInfoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ContactInfo> AddContactInfoAsync(ContactInfo contactInfo)
        {
            return await _repository.AddContactInfoAsync(contactInfo);
        }
    }
}
