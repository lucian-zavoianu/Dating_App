using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    // [ApiController]

    // Using ApiController data annotation helps with the validation process by:
    // - not needing [FromBody] specification for the DTO
    // - not needing the !ModelState.IsValid conditional for validation
    // If using [ApiController], the above conditions can be removed
    public class AuthController : ControllerBase

    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserToRegisterDto userToRegisterDto) {
            // Validate Request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // User name conversion to lowercase, to avoid duplicates where
            // the only difference might be a lowercase vs. uppercase letter
            userToRegisterDto.Username = userToRegisterDto.Username.ToLower();

            // Check if user exists
            if (await _repo.UserExists(userToRegisterDto.Username))
                return BadRequest("Username already exists!");

            var userToCreate = new User {
                Username = userToRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userToRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserToLoginDto userToLoginDto) {
            // Check if user exists
            var userFromRepo = await _repo.Login(userToLoginDto.Username.ToLower(), userToLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();
            
            // Create the claims
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // In order to make sure that the token is valid when it comes back, the server needs to sign the token
            // Create security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Using the key as part of the signing credentials and we encrypting it with a hashing algorythm
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Start creating the token
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}