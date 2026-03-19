using HRManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        // TEMP: Hardcoded users (Phase 1)
        if (login.Username == "admin" && login.Password == "123")
        {
            var token = GenerateToken("Admin");
            return Ok(new { token });
        }

        if (login.Username == "hr" && login.Password == "123")
        {
            var token = GenerateToken("HR");
            return Ok(new { token });
        }

        if (login.Username == "user" && login.Password == "123")
        {
            var token = GenerateToken("User");
            return Ok(new { token });
        }

        return Unauthorized();
    }

    private string GenerateToken(string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("THIS_IS_MY_SUPER_SECRET_KEY_1234567890"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}