namespace HRIS.API.Services.Email;

public class MailRequest
{
    public required string ToEmail { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public string? TextBody { get; init; }   // plain-text alternative for clients that cannot show HTML
}