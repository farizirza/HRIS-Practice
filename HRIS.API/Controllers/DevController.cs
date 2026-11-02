using System.ComponentModel.DataAnnotations;
using HRIS.API.Services.Email;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/dev")]
public class DevController(IEmailService emailService, IEmailTemplateRenderer renderer, IWebHostEnvironment env) : ControllerBase
{
    public record TestEmailRequest([Required, EmailAddress] string To, [Required] string FullName);

    [HttpPost("test-email")]
    public async Task<IActionResult> TestEmail(TestEmailRequest request, CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return NotFound();

        var html = await renderer.RenderAsync("WelcomeEmployee", new Dictionary<string, string>
        {
            ["FullName"] = request.FullName,
            ["EmployeeNumber"] = "EMP-0000",
            ["ActivationUrl"] = "https://localhost:5173/activate"
        }, ct);

        try
        {
            await emailService.SendEmailAsync(new MailRequest
            {
                ToEmail = request.To,
                Subject = $"HRIS test email for {request.FullName}",
                HtmlBody = html,
                TextBody = $"Welcome, {request.FullName}. Open this email in an HTML-capable client to activate your account."
            }, ct);
        }
        catch (EmailDeliveryException ex)
        {
            return Problem(statusCode: StatusCodes.Status502BadGateway, title: "The email could not be delivered.", detail: ex.Message);
        }

        return NoContent();
    }
}