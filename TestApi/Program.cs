using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using TestApi.Application.Users;
using TestApi.Domain.Interfaces;
using TestApi.Identity;
using TestApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste a JWT here as: Bearer {token}"
    };

    c.AddSecurityDefinition("Bearer", bearerScheme);

    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", null!, null),
            new List<string>()
        }
    });
});
builder.Services.AddHttpClient();

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"] ?? "https://localhost:5001";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services
    .AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients)
    .AddTestUsers(TestUsers.Users)
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();