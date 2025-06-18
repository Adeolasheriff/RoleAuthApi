using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Dto;
using Auth.Entities;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
      
        [HttpPost("Register")]

        public async Task< ActionResult<User>> Register(UserDto userDto)
        {
            var user = await authService.RegisterAsync(userDto);
            if (user == null)
            {
                return BadRequest("User already exists");
            }

            return Ok(user);

        }

        [HttpPost("Login")]

        public async  Task< ActionResult<String>> Login (UserDto userDto)
        {
            var token = await authService.LoginAsync(userDto);
            if (token == null)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(token);

        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoints()
        {
            return Ok("You are authenticated");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("You are an admin");

        }

    }
}