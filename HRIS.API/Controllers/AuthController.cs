using System.Security.Claims;
using HRIS.API.Common.Security;
using HRIS.API.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

public record LoginRequest(string Email, string Password);
public record LoginResult(Guid UserId, Guid? EmployeeId, string Email, IReadOnlyCollection<string> Roles);

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender, IUserCredentialService credentialService) : ControllerBase
{
    /// <summary>Activate an account for an existing employee.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
        => StatusCode(StatusCodes.Status201Created, await sender.Send(command, ct));

    /// <summary>Check an email and password once. Module 11 turns this into "issue a token".</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await credentialService.ValidateAsync(request.Email, request.Password, ct);
        if (user is null)   // the same answer for "unknown email" and "wrong password": do not reveal which emails exist
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid email or password.");

        return Ok(new LoginResult(user.Id, user.EmployeeId, user.Email, user.Roles));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]   // every action needs an authenticated user
public class ProfileController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        Email = User.FindFirst(ClaimTypes.Email)?.Value,
        Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value),
        EmployeeId = User.FindFirst(HrisClaimTypes.EmployeeId)?.Value   // null for accounts without an employee record
    });
}