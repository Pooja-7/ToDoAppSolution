using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            // Validate the token
            await AttachUserToContext(context, token);
        }

        await _next(context); // Call the next middleware in the pipeline
    }

    private async Task AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes("your_super_secret_key_here"); // Ensure this matches your JWT secret key
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // Remove delay of token when expire
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "sub").Value; // Get the user ID from the token claims

            // Attach the user to the context on successful jwt validation
            context.Items["User"] = userId; // You can also create a ClaimsPrincipal here if needed
        }
        catch
        {
            // Token validation failed, do nothing or handle error
        }
    }
}

// Extension method to add the middleware to the pipeline
public static class JwtMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
}
