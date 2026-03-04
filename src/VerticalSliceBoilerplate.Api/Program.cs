using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using VerticalSliceBoilerplate.Api.Auth;
using VerticalSliceBoilerplate.Api.Features.Auth;
using VerticalSliceBoilerplate.Api.Features.Sample;
using VerticalSliceBoilerplate.Core.Features.Auth;
using VerticalSliceBoilerplate.Core.Features.Auth.Services;
using VerticalSliceBoilerplate.Core.Features.Sample;
using VerticalSliceBoilerplate.Infrastructure.Data;
using VerticalSliceBoilerplate.Infrastructure.Data.Context;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (e.g. Bearer &lt;token&gt;)"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Infrastructure (includes domain event dispatcher and interceptors)
builder.Services.AddDataInfrastructure(builder.Configuration);

// Identity
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// JWT authentication
builder.Services.AddJwtAuth(builder.Configuration, builder.Environment);

// JWT token generation (used by Login handler)
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole(Roles.Admin));

// Core features (handlers and validators)
builder.Services.AddSampleFeatureCore();
builder.Services.AddAuthFeatureCore();

// TODO declare to add as feature including endpoints and core. Is it good?
// API endpoints
builder.Services.AddSampleEndpoints();
builder.Services.AddAuthEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "VerticalSliceBoilerplate is running.");

app.MapEndpoints();

app.Run();
