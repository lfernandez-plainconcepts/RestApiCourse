using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), // Not secure. It is just for learning purposes.
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["Jwt:Audience"],
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.Policies.Admin, p => p.RequireClaim(AuthConstants.Claims.Admin, "true"))
    .AddPolicy(AuthConstants.Policies.TrustedMember, p => p.RequireAssertion(c =>
        c.User.HasClaim(m => m is { Type: AuthConstants.Claims.Admin, Value: "true" }) ||
        c.User.HasClaim(m => m is { Type: AuthConstants.Claims.TrustedMember, Value: "true" })));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

if (bool.TryParse(config["Database:SeedOnStartup"], out bool seedOnStartup) && seedOnStartup)
{
    await dbInitializer.SeedAsync();
}

await app.RunAsync();

