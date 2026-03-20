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
            //return Ok(new { token });
            // Set HTTP-only cookie
            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,        // JavaScript cannot access
                Secure = true,          // only sent over HTTPS
                SameSite = SameSiteMode.Strict, // adjust for your frontend domain
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            return Ok(new { message = "ADMIN Login successful" , role = "ADMIN" , token });
        }

        if (login.Username == "hr" && login.Password == "123")
        {
            var token = GenerateToken("HR");
            //return Ok(new { token });
            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,        // JavaScript cannot access
                Secure = true,          // only sent over HTTPS
                SameSite = SameSiteMode.None, // adjust for your frontend domain
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            return Ok(new { message = "HR Login successful", role = "HR", token });
        }

        if (login.Username == "user" && login.Password == "123")
        {
            var token = GenerateToken("User");
            //return Ok(new { token });
            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,        // JavaScript cannot access
                Secure = true,          // only sent over HTTPS
                SameSite = SameSiteMode.Strict, // adjust for your frontend domain
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            return Ok(new { message = "USER Login successful", role = "USER", token });
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwtToken");
        return Ok(new { message = "Logged out successfully" });
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