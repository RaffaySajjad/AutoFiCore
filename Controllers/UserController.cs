using AutoFiCore.Models;
using AutoFiCore.Services;
using Microsoft.AspNetCore.Mvc;
using AutoFiCore.Utilities;

namespace AutoFiCore.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserController:ControllerBase
    {
        private readonly IUserService _userService;

        public UserController (IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("add")]

        public async Task<ActionResult<User>> AddUserInfo([FromBody] User user)
        {
            try
            {
                var errors = Validator.ValidateUserInfo(user);

                if (errors.Any())
                {
                    return BadRequest(new { errors });
                }

                var addedUser = await _userService.AddUserAsync(user);
                return Ok(addedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving user info.",
                    error = ex.Message
                });
            }
        }


    }
}
