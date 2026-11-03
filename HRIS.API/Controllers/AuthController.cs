using System.Security.Claims;
using HRIS.API.Common.Security;
using HRIS.API.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

public record LoginRequest(string Email, string Password);

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender, IUserCredentialService credentialService, ITokenService tokenService) : ControllerBase
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

    /// <summary>Check an email and password and issue a JWT access token.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        // 1. Validate the credentials (hash check, active account) with the Module 10 service
        var user = await credentialService.ValidateAsync(request.Email, request.Password, ct);
        if (user is null)   // the same answer for "unknown email" and "wrong password": do not reveal which emails exist
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid email or password.");

        // 2. Create the JWT and 3. return it
        var (token, expiresAt) = tokenService.CreateToken(user);
        return Ok(new LoginResponse(token, expiresAt));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]   // every action needs an authenticated user
public class ProfileController(ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        Email = User.FindFirst(ClaimTypes.Email)?.Value,
        Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value),
        EmployeeId = currentUser.EmployeeId
    });
}