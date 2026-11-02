using System.Security.Claims;
using HRIS.API.Common.Exceptions;

namespace HRIS.API.Common.Security;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid? EmployeeId { get; }   // null for accounts without an employee record (for example the bootstrap Admin)
    bool IsInRole(string role);
    bool IsHrOrAdmin => IsInRole(Roles.Hr) || IsInRole(Roles.Admin);
}

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal User => accessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id
        : throw new UnauthorizedException("No authenticated user.");   // 401, not 500

    public Guid? EmployeeId => Guid.TryParse(User.FindFirstValue(HrisClaimTypes.EmployeeId), out var id) ? id : null;

    public bool IsInRole(string role) => User.IsInRole(role);
}