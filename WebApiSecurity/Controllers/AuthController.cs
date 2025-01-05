using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApiSecurity.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public IConfiguration Config { get; }

    public AuthController(IConfiguration config)
    {
        Config = config;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (credential is { Username: "admin", Password: "password" })
        {
            // Create security context
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "admin"),
                new(ClaimTypes.Email, "admin@mywebsite.com"),
                new("Department", "HR"),
                new("Admin", "true"),
                new("Manager", "true"),
                new("EmploymentDate", "2024-05-01"),
            };

            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            return Ok(new
            {
                access_token = CreateToken(claims, expiresAt),
                expires_at = expiresAt
            });
        }

        ModelState.AddModelError("Unauthorized", "You are not authorized to access this endpoint.");
        return Unauthorized(ModelState);
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var secretKey = Encoding.ASCII.GetBytes(Config.GetValue<string>("SecretKey") ?? string.Empty);
        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}