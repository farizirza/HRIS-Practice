using MailKit.Security;

namespace HRIS.API.Services.Email;

public class MailSettings
{
    public const string SectionName = "MailSettings";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 2525;

    // smtp4dev / Mailpit: None.  Gmail or Office 365 on port 587: StartTls.  Port 465: SslOnConnect.
    public SecureSocketOptions Security { get; set; } = SecureSocketOptions.Auto;

    public string SenderName { get; set; } = "HRIS";
    public string SenderEmail { get; set; } = "no-reply@hris.local";

    // Leave UserName empty for servers without authentication (smtp4dev).
    // The password is NEVER in appsettings.json: use User Secrets (development) or environment variables (production).
    public string? UserName { get; set; }
    public string? Password { get; set; }
}