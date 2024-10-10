using Microsoft.AspNetCore.Identity.Data;

public interface IAuthService
{
    Task<AuthResponse> CreateUserAsync(CreateUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    User ValidateToken(string token);


}