using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly string _jwtSecret; //  JWT secret key
    private readonly string _jwtIssuer; //  JWT issuer

    public AuthController(IAuthService authService)
    {
        _authService = authService;
        // Hardcoded for testing
        _jwtSecret = "a_very_long_secret_key_that_is_at_least_32_characters"; 
        _jwtIssuer = "Todo";
    }

    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(CreateUserRequest request)
    {
        var response = await _authService.CreateUserAsync(request);
        // Error number is non-zero for failure, 
        // added custom error numbers 
        //so that front end can handle the error responses well
        //Also comes handy in troubleshooting
        if (response.ErrorNumber != 0) 
        {
            return BadRequest(new { response.ErrorNumber, response.ErrorDescription });
        }

        // Generate JWT token for the newly registered user
        // Make sure UserId is part of the AuthResponse, needs userId to be send on subsequent calls,
        // Manage user state
        var token = GenerateJwtToken(response.UserId); 

        return Ok(new
        {
            Message = "Registration successful!",
            Token = token // Return the JWT token along with the response
        });
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (!response.Success)
        {
            return Unauthorized(new { response.ErrorNumber, response.ErrorDescription });
        }

        var token = GenerateJwtToken(response.UserId); 

        return Ok(new
        {
            Message = "Login successful!",
            Token = token
        });
    }

    private string GenerateJwtToken(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
        }

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddHours(1).ToString()), // Set expiration to 1 hour
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtIssuer,
            claims: claims,
            expires: DateTime.Now.AddHours(1), // Token expiration time
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token); // Return the serialized token
    }
}
