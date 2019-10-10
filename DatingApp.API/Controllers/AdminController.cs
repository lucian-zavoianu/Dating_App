using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public AdminController(DataContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public IActionResult GetUsersWithRoles()
        {
            return Ok("Only Admins can see this.");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosForModeration()
        {
            return Ok("Admins or Moderators can see this.");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;

            // selected = selectedRoles != null ? selectedRoles : new string[] {};
            selectedRoles = selectedRoles ?? new string[] {};

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to add to roles.");
            
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to remove the roles.");

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}