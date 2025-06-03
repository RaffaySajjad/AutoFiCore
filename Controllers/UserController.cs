using AutoFiCore.Models;
using AutoFiCore.Services;
using Microsoft.AspNetCore.Mvc;
using AutoFiCore.Utilities;
using AutoFiCore.Dto;

namespace AutoFiCore.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserController:ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVehicleService _vehicleService;

        public UserController (IUserService userService, IVehicleService vehicleService)
        {
            _userService = userService;
            _vehicleService = vehicleService;
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
                if (addedUser == null) {
                    return Conflict(new { message = "Email already exists. " });
                }
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

        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var user = await _userService.LoginUserAsync(loginDTO.Email, loginDTO.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }
                return Ok(user);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }

        [HttpPost("add-user-like")]
        public async Task<ActionResult<UserLikes>> AddUserLike([FromBody] UserLikes userLikes)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userLikes.userId);
                var vehicle = await _vehicleService.GetVehicleByVinAsync(userLikes.vehicleVin);

                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {userLikes.userId} not found." });
                }

                if (vehicle == null)
                {
                    return NotFound(new { message = $"Vehicle with VIN {userLikes.vehicleVin} not found." });
                }

                var addedLike = await _userService.AddUserLikeAsync(userLikes);
                return Ok(addedLike);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding user like", error = ex.Message });
            }
        }

        [HttpDelete("remove-user-like")]
        public async Task<ActionResult<UserLikes>> RemoveUserLike([FromBody] UserLikes userLikes)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userLikes.userId);
                var vehicle = await _vehicleService.GetVehicleByVinAsync(userLikes.vehicleVin);

                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {userLikes.userId} not found." });
                }

                if (vehicle == null)
                {
                    return NotFound(new { message = $"Vehicle with VIN {userLikes.vehicleVin} not found." });
                }

                var removedLike = await _userService.RemoveUserLikeAsync(userLikes);
                return Ok(removedLike);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding user like", error = ex.Message });
            }
        }

        [HttpGet("get-user-liked-vins/{id}")]
        public async Task<ActionResult<List<string>>> GetUserLikedVins(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return await _userService.GetUserLikedVinsAsync(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching user liked vins ", error = ex.Message });
            }
        }
       

    }
}
