using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserToRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "The password must me between 8 and 15 characters long.")]
        public string Password { get; set; }
    }
}