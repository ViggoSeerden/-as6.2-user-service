using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService;
using UserServiceBusiness.Interfaces;
using UserServiceBusiness.Services;
using UserServiceDAL.DataContext;
using UserServiceDAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database Context.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ??
                      builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<UserServiceBusiness.Services.UserService>();
builder.Services.AddScoped<RoleService>();

// Auth0

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = Environment.GetEnvironmentVariable("Auth__Domain");
    options.Audience = Environment.GetEnvironmentVariable("Auth__Audience");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
    if (builder.Environment.IsDevelopment())
    {
        options.RequireHttpsMetadata = false;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token validation failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token successfully validated.");
                return Task.CompletedTask;
            }
        };
    }
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:users", policy => policy.Requirements.Add(new
        HasScopeRequirement("read:users", Environment.GetEnvironmentVariable("Auth__Domain"))));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// Debugging
Console.WriteLine(builder.Environment.IsDevelopment());
// Console.WriteLine(Environment.GetEnvironmentVariable("Auth__Audience"));

// Cors
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development", policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
    }
    // else
    // {
    // }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHostedService<MessageReceiver>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

if (builder.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
// else
// {
// }

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

public partial class Program
{
}