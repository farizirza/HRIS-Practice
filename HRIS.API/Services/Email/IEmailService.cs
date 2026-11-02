namespace HRIS.API.Services.Email;

public interface IEmailService
{
    /// <exception cref="EmailDeliveryException">The SMTP server could not be reached, rejected the login, or rejected the message.</exception>
    Task SendEmailAsync(MailRequest request, CancellationToken ct = default);
}