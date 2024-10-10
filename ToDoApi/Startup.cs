using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
            builder => builder.WithOrigins("http://localhost:3000") // Your React app URL
                              .AllowAnyHeader()
                              .AllowAnyMethod());
});

// Register DbContext for dependency injection
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your ToDoService and AuthService for dependency injection
builder.Services.AddScoped<IToDoService, ToDoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();


// Hardcode the JWT secret key
var key = Encoding.ASCII.GetBytes("your_very_long_secret_key_that_is_at_least_32_characters");

// Add JWT authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Register the Swagger generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });

    // Add a security definition for the JWT Bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Add a security requirement to the Swagger UI
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
// Use CORS policy
app.UseCors("AllowSpecificOrigin");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>(); // Ensure JwtMiddleware is correctly implemented

app.MapControllers();

app.Run();
