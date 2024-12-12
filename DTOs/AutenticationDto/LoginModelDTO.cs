using System.ComponentModel.DataAnnotations;

namespace ASP.NETCore_WebAPI.DTOs.AutenticationDto
{
    public class LoginModelDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
