using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Cache;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Movies.Contracts.Requests;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
var config = builder.Configuration;

// Add services to the container.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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

builder.Services
    .AddAuthorizationBuilder()
    //.AddPolicy(AuthConstants.Policies.Admin, p => p.RequireClaim(AuthConstants.Claims.Admin, "true"))
    .AddPolicy(AuthConstants.Policies.Admin, p => p.AddRequirements(new AdminAuthRequirement(config)))
    .AddPolicy(AuthConstants.Policies.TrustedMember, p => p.RequireAssertion(c =>
        c.User.HasClaim(m => m is { Type: AuthConstants.Claims.Admin, Value: "true" }) ||
        c.User.HasClaim(m => m is { Type: AuthConstants.Claims.TrustedMember, Value: "true" })));

builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services
    .AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddResponseCaching();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(c => c.Cache());
    options.AddPolicy(CacheConstants.Policies.Movies, policy =>
    {
        policy.Cache()
            .Expire(TimeSpan.FromMinutes(1))
            .SetVaryByQuery([
                .. RequestMoviesFilterParams.ValidFilterFields,
                .. RequestPageParams.ValidPageFields,
                .. RequestSortParams.ValidSortFields])
            .Tag(CacheConstants.Tags.Movies);
    });
});

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var apiGroupNames = app.DescribeApiVersions()
            .Select(v => v.GroupName);

        foreach (var apiGroupName in apiGroupNames)
        {
            options.SwaggerEndpoint($"/swagger/{apiGroupName}/swagger.json", apiGroupName);
        }
    });
}

app.MapHealthChecks("_health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Caching must be used after configuring CORS

app.UseResponseCaching();

// By default:
// - only 200 responses are cached.
// - only GET and HEAD requests are cached.
// - responses that set cookies are not cached.
// - response to authenticated requests are not cached.
app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

if (bool.TryParse(config["Database:SeedOnStartup"], out bool seedOnStartup) && seedOnStartup)
{
    await dbInitializer.SeedAsync();
}

await app.RunAsync();

