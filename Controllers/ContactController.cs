using AutoFiCore.Models;
using AutoFiCore.Services;
using AutoFiCore.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AutoFiCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactInfoService _contactInfoService;

        public ContactController(IContactInfoService contactInfoService)
        {
            _contactInfoService = contactInfoService;
        }

        [HttpPost("add")]

        public async Task<ActionResult<ContactInfo>> AddContactInfo([FromBody] ContactInfo contactInfo)
        {
            try
            {
                var errors = Validator.ValidateContactInfo(contactInfo);

                if (errors.Any())
                {
                    return BadRequest(new { errors });
                }

                var addedContact = await _contactInfoService.AddContactInfoAsync(contactInfo);
                return Ok(addedContact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving contact info.",
                    error = ex.Message
                });
            }
        }

    }
}
