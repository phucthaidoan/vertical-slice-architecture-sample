using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VerticalSliceBoilerplate.Core.Features.Auth.Login;
using VerticalSliceBoilerplate.Core.Features.Auth.Services;

namespace VerticalSliceBoilerplate.Api.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;

    public JwtTokenService(IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<LoginResponse> GenerateAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        var jwtSection = _configuration.GetSection("Authentication:Jwt");
        var signingKeyValue = jwtSection["SigningKey"] ?? throw new InvalidOperationException("JWT SigningKey not configured.");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyValue));
        var issuer = jwtSection["Issuer"] ?? "VerticalSliceBoilerplate";
        var audience = jwtSection["Audience"] ?? "VerticalSliceBoilerplate";
        var expiresMinutes = 60;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse
        {
            Token = tokenString,
            ExpiresAtUtc = expiresAt
        };
    }
}
