using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.AutenticationDto;
using ASP.NETCore_WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ASP.NETCore_WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.Email, user.Email),
                new ("id", user.UserName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                                            out int refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);

        if (userExists is not null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User already exists!" });
        }

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User creation failed." });
        }

        return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    {

        if (tokenModel is null)
            return BadRequest("Invalid client request");

        string accessToken = tokenModel.AccesssToken ?? throw new ArgumentNullException(nameof(tokenModel));

        string refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken, _configuration);

        if (principal is null)
            return BadRequest("Invalid access token/refresh token");

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Invalid access token/refresh token");

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken,
        });
    }

    [HttpPost]
    [Route("revoke/{username}")]
    [Authorize(Policy ="ExclusiveOnly")] 
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByIdAsync(username);

        if (user is null)
            return BadRequest("Invalid user name");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy ="SuperAdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);

        if (roleExist)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
               new ResponseDTO { Status = "Error", Message = "Role already exist." });
        }

        var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (!roleResult.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new ResponseDTO { Status = "Error", Message = "Issue adding the new role." });
        }

        _logger.LogInformation(1, "Roles Added");

        return StatusCode(StatusCodes.Status200OK,
            new ResponseDTO { Status = "Succes", Message = $"Role {roleName} added succesfully" });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        string message = "Unable to find user";
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                message = $"User {user.Email} added to the {roleName} role";

                _logger.LogInformation(1, "User {user} added to the {roleName} role", email, roleName);

                return StatusCode(StatusCodes.Status200OK,
                    new ResponseDTO { Status = "Success", Message = message });
            }
            else
            {
                message = $"Error: Unable to add user {user.Email} to the {roleName} role";

                _logger.LogInformation(1, "Error: Unable to add user {user.Email} to the {roleName} role", email, roleName);

                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseDTO { Status = "Error", Message= message });
            }
        }

        return BadRequest(error: message);
    }
}