using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using HRIS.API.Services.Email;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

/// <summary>Module 9 helper: sends one test email. Development only.</summary>
[ApiController]
[Route("api/dev")]
public class DevController(IEmailService emailService, IWebHostEnvironment env) : ControllerBase
{
    public record TestEmailRequest([Required, EmailAddress] string To, [Required] string FullName);

    [HttpPost("test-email")]
    public async Task<IActionResult> TestEmail(TestEmailRequest request, CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return NotFound();   // the endpoint does not exist outside Development

        var name = HtmlEncoder.Default.Encode(request.FullName);   // never put raw user input into HTML
        try
        {
            await emailService.SendEmailAsync(new MailRequest
            {
                ToEmail = request.To,
                Subject = $"HRIS test email for {request.FullName}",
                HtmlBody = $"<h1>Hello {name}</h1><p>This is a test email from <strong>HRIS</strong>.</p>",
                TextBody = $"Hello {request.FullName}, this is a test email from HRIS."
            }, ct);
        }
        catch (EmailDeliveryException ex)
        {
            // Development only, so showing the reason helps you fix MailSettings. Never do this in a production endpoint.
            return Problem(statusCode: StatusCodes.Status502BadGateway, title: "The email could not be delivered.", detail: ex.Message);
        }

        return NoContent();
    }
}