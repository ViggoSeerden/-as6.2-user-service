using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("read:users", policy => policy.Requirements.Add(new
        HasScopeRequirement("read:users", Environment.GetEnvironmentVariable("Auth__Domain"))))
    .AddPolicy("read:user", policy => policy.Requirements.Add(new
        HasScopeRequirement("read:user", Environment.GetEnvironmentVariable("Auth__Domain"))))
    .AddPolicy("read:self", policy => policy.Requirements.Add(new
        HasScopeRequirement("read:self", Environment.GetEnvironmentVariable("Auth__Domain"))))
    .AddPolicy("write:add_user", policy => policy.Requirements.Add(new
        HasScopeRequirement("write:add_user", Environment.GetEnvironmentVariable("Auth__Domain"))))
    .AddPolicy("write:update_user", policy => policy.Requirements.Add(new
        HasScopeRequirement("write:update_user", Environment.GetEnvironmentVariable("Auth__Domain"))))
    .AddPolicy("write:delete_user", policy => policy.Requirements.Add(new
        HasScopeRequirement("write:delete_user", Environment.GetEnvironmentVariable("Auth__Domain"))));

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// Debugging
Console.WriteLine(builder.Environment.EnvironmentName);
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
    else
    {
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins("http://localhost:3000"); //temp
            policy.AllowAnyHeader();
            policy.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
        });
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

try
{
    ConnectionFactory factory = new()
        { HostName = Environment.GetEnvironmentVariable("RabbitMQ") ?? "localhost" };

    IConnection conn = await factory.CreateConnectionAsync();
    IChannel channel = await conn.CreateChannelAsync();

    builder.Services.AddSingleton(channel);
    builder.Services.AddScoped<MessageProducer>();
    builder.Services.AddHostedService<MessageReceiver>();
} catch (Exception e)
{
    Console.WriteLine("Error connecting to RabbitMQ");
    Console.WriteLine(e.Message);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder.Environment.IsDevelopment() ? "Development" : "Production");

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    Seeder.SeedData(dbContext);
}

app.Run();

public partial class Program
{
}