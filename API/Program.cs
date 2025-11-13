using Core.Interfaces;
using Core.Services;
using Core.Utilities.Filters;
using Core.Utilities.Services;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Create Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/logs-.txt", rollingInterval: RollingInterval.Infinite)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();
//

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<AuditLogsCleanService>();
builder.Services.AddScoped<LogActionFilter>();
//

// Load connection string
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
//

// Register Repositories
builder.Services.AddScoped<IUserRepo>(_ => new UserRepo(connectionString));
builder.Services.AddScoped<IVaultRepo>(_ => new VaultRepo(connectionString));
builder.Services.AddScoped<IVaultItemsRepo>(_ => new VaultItemsRepo(connectionString));
builder.Services.AddSingleton<IAuditLogsRepo>(_ => new AuditLogsRepo(connectionString));
//

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVaultService, VaultService>();
builder.Services.AddScoped<IVaultItemsService, VaultItemsService>();

builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
//

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
//

// Build the app
var app = builder.Build();
//


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//

app.Run();
