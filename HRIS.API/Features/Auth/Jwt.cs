using System.Security.Claims;
using System.Text;
using HRIS.API.Common.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HRIS.API.Features.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Key { get; set; } = "";        // at least 32 bytes; from User Secrets / environment variables, never from Git
    public int ExpiresMinutes { get; set; } = 60;
}

/// <summary>The access token and when it stops being valid.</summary>
public record LoginResponse(string AccessToken, DateTime ExpiresAt, string TokenType = "Bearer");

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(AuthenticatedUser user);
}

public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider time) : ITokenService
{
    public (string Token, DateTime ExpiresAt) CreateToken(AuthenticatedUser user)
    {
        var jwt = options.Value;
        var now = time.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(jwt.ExpiresMinutes);

        // Claim names from RFC 7519 ("sub", "jti", ...) plus our own ("employee_id").
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (user.EmployeeId is { } employeeId)
            claims.Add(new Claim(HrisClaimTypes.EmployeeId, employeeId.ToString()));   // the bootstrap Admin has no employee record
        claims.AddRange(user.Roles.Select(role => new Claim("role", role)));          // one claim per role

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = now,
            IssuedAt = now,
            Expires = expiresAt,
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);   // returns the finished "header.payload.signature" string
        return (token, expiresAt);
    }
}