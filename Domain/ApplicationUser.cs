using Microsoft.AspNetCore.Identity;

namespace ASP.NETCore_WebAPI.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
