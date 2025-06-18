using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Data;
using Auth.Dto;
using Auth.Entities;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services
{
    public class AuthService(AppDbContext dbContext, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(UserDto userDto)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null)
            {
                return null; // User not found
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userDto.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }


            return CreateToken(user);
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                  new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                  new Claim(ClaimTypes.Role, user.Role) // Assuming the User entity has a Role property


            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new
                 JwtSecurityToken(

                issuer: configuration.GetValue<string>("AppSettings:Issuer")
                , audience: configuration.GetValue<string>("AppSettings:Audience")
                , claims: claims
                , expires: DateTime.Now.AddDays(1)
                , signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            if (await dbContext.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return null; // User already exists
            }
            var user = new User();

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, userDto.Password);
            user.Username = userDto.Username;
            user.PasswordHash = hashedPassword;

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user; // Return the newly created user


        }

     
    }
}
