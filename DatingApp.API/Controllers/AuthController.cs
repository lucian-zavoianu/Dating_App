using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    // [ApiController]
    [AllowAnonymous]

    // Using ApiController data annotation helps with the validation process by:
    // - not needing [FromBody] specification for the DTO
    // - not needing the !ModelState.IsValid conditional for validation
    // If using [ApiController], the above conditions can be removed
    public class AuthController : ControllerBase

    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserToRegisterDto userToRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userToRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userToRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailDto>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new
                {
                    controller = "Users",
                    id = userToCreate.Id
                }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserToLoginDto userToLoginDto)
        {
            // Check if user exists
            var user = await _userManager.FindByNameAsync(userToLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userToLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appuser = _mapper.Map<UserForListDto>(user);

                return Ok(new
                {
                    token = GenerateJwtToken(user),
                    user = appuser
                });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            // Create the claims
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // In order to make sure that the token is valid when it comes back, the server needs to sign the token
            // Create security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Using the key as part of the signing credentials and we encrypting it with a hashing algorythm
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Start creating the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}