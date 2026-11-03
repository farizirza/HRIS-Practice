using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;
using HRIS.API.Features.Auth;
using Xunit;

namespace HRIS.API.Tests;

public class JwtTokenServiceTests
{
    private static readonly JwtOptions TestJwt = new()
    {
        Issuer = "HRIS.API",
        Audience = "HRIS.API.Clients",
        Key = "unit-test-key-unit-test-key-unit-test-key-1234",   // 32+ bytes, test value only
        ExpiresMinutes = 60
    };

    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero));
    private readonly JwtTokenService _sut;

    public JwtTokenServiceTests() => _sut = new JwtTokenService(Options.Create(TestJwt), _time);

    private static AuthenticatedUser User(Guid? employeeId, params string[] roles)
        => new(Guid.Parse("0199f000-0000-7000-8000-000000000001"), "siti@example.com", "Siti", employeeId, roles);

    [Fact]
    public void CreateToken_SetsIssuerAudienceAndExpiryFromTheClock()
    {
        var (token, expiresAt) = _sut.CreateToken(User(null, "Employee"));

        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);   // reads the token WITHOUT validating it
        Assert.Equal("HRIS.API", jwt.Issuer);
        Assert.Contains("HRIS.API.Clients", jwt.Audiences);
        Assert.Equal(new DateTime(2026, 10, 19, 10, 0, 0, DateTimeKind.Utc), expiresAt);
        Assert.Equal(expiresAt, jwt.ValidTo);
    }

    [Fact]
    public void CreateToken_IncludesRoleClaimsForEachRole()
    {
        var (token, _) = _sut.CreateToken(User(null, "Employee", "Manager"));

        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);
        var roles = jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        Assert.Contains("Employee", roles);
        Assert.Contains("Manager", roles);
    }

    [Fact]
    public void CreateToken_IncludesEmployeeIdOnlyWhenUserHasEmployeeRecord()
    {
        var empId = Guid.NewGuid();
        var (tokenWithEmp, _) = _sut.CreateToken(User(empId, "Employee"));
        var (tokenWithoutEmp, _) = _sut.CreateToken(User(null, "Admin"));

        var jwtWithEmp = new JsonWebTokenHandler().ReadJsonWebToken(tokenWithEmp);
        var jwtWithoutEmp = new JsonWebTokenHandler().ReadJsonWebToken(tokenWithoutEmp);

        Assert.Equal(empId.ToString(), jwtWithEmp.GetClaim("employee_id").Value);
        Assert.Null(jwtWithoutEmp.Claims.FirstOrDefault(c => c.Type == "employee_id"));
    }
}
