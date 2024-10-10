using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class AuthService : IAuthService
{
    private readonly ToDoContext _context;
    private readonly string _secretKey;


    public AuthService(ToDoContext context)
    {
        _context = context;
        _secretKey = "your_secret_key_here"; // Same key used in ConfigureServices

    }

    public async Task<AuthResponse> CreateUserAsync(CreateUserRequest request)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return new AuthResponse(1000, "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return new AuthResponse(1001, "Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
        {
            return new AuthResponse(1002, "A valid email address is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthResponse(1003, "Password is required.");
        }

        // Check for existing user
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return new AuthResponse(400, "User with this email already exists.");
        }

        var userId = Guid.NewGuid().ToString();

        // Create new user
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserId = userId,
            Password = HashPassword(request.Password) // Hash the password using BCrypt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse(true, userId); // Successful response
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
        {
            return new AuthResponse(1002, "A valid email address is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthResponse(1003, "Password is required.");
        }

        try
        {
            // Check for existing user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new AuthResponse(2000, "Invalid email or password."); // Invalid email
            }

            if (!VerifyPassword(request.Password, user.Password))
            {
                return new AuthResponse(2001, "Invalid email or password."); // Invalid password
            }
            var userId = user.UserId;

            // On successful authentication, return a success response
            return new AuthResponse(true, userId); 
        }
        catch (Exception ex)
        {
            // Log the exception (optional)
            // Log.Error(ex, "An error occurred during login.");

            return new AuthResponse(2002, "An error occurred while processing your request."); // General error
        }
    }

    public User ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return _context.Users.FirstOrDefault(u => u.Email == principal.Identity.Name); // Assuming Email is the claim
        }
        catch
        {
            return null; // Token is invalid
        }
    }



    private string HashPassword(string password)
    {
        // Hash the password using BCrypt
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    protected bool IsValidEmail(string email)
    {
        // Basic email validation using regex
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    protected bool VerifyPassword(string password, string hashedPassword)
    {
        // Verify the password using BCrypt
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
