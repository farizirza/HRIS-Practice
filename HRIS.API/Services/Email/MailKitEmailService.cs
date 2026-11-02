using System.Net.Sockets;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HRIS.API.Services.Email;

public class MailKitEmailService(IOptions<MailSettings> options, ILogger<MailKitEmailService> logger) : IEmailService
{
    private readonly MailSettings _settings = options.Value;

    public async Task SendEmailAsync(MailRequest request, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(request.ToEmail));
        message.Subject = request.Subject;
        message.Body = new BodyBuilder
        {
            HtmlBody = request.HtmlBody,
            TextBody = request.TextBody ?? "Please open this email in an HTML-capable email client."
        }.ToMessageBody();   // multipart/alternative: a text/plain part and a text/html part

        using var smtp = new SmtpClient();   // "using": the connection is disposed even when an exception is thrown
        try
        {
            // ALWAYS call ConnectAsync. The security mode comes from settings, so the same code works with
            // smtp4dev (None) and with a real provider (StartTls / SslOnConnect).
            await smtp.ConnectAsync(_settings.Host, _settings.Port, _settings.Security, ct);

            if (!string.IsNullOrEmpty(_settings.UserName))   // smtp4dev has no authentication
                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password ?? "", ct);

            await smtp.SendAsync(message, ct);
            await smtp.DisconnectAsync(true, ct);
        }
        // Translate MailKit and network exceptions into OUR exception and keep the original as InnerException.
        // This class does not log the failure: the caller logs it once, with the business context (which employee).
        catch (AuthenticationException ex)
        {
            throw new EmailDeliveryException($"SMTP authentication failed for host '{_settings.Host}'. Check MailSettings.", ex);
        }
        catch (NotSupportedException ex)   // the server lacks STARTTLS or AUTH although MailSettings asks for it
        {
            throw new EmailDeliveryException(
                $"The SMTP server {_settings.Host}:{_settings.Port} does not support the requested security or login mode. Check MailSettings:Security and MailSettings:UserName.", ex);
        }
        catch (SmtpCommandException ex)
        {
            throw new EmailDeliveryException($"The SMTP server rejected the message (error {ex.ErrorCode}, status {ex.StatusCode}).", ex);
        }
        catch (Exception ex) when (ex is SmtpProtocolException or SslHandshakeException or SocketException or IOException)
        {
            throw new EmailDeliveryException($"The SMTP server {_settings.Host}:{_settings.Port} could not be reached.", ex);
        }
        // OperationCanceledException is not caught: cancellation must travel up unchanged.

        // No recipient in the log: an email address is personal data.
        logger.LogDebug("Email with subject {Subject} sent through {SmtpHost}", request.Subject, _settings.Host);
    }
}