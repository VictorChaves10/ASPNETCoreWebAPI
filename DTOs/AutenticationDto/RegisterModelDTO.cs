using System.ComponentModel.DataAnnotations;

namespace ASP.NETCore_WebAPI.DTOs.AutenticationDto
{
    public class RegisterModelDTO
    {
        [Required]
        public string Username { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
