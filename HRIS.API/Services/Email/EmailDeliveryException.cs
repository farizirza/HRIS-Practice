namespace HRIS.API.Services.Email;

/// <summary>
/// Thrown when an email could not be delivered. Callers only need to know this one type (not MailKit's types),
/// and InnerException keeps the original SMTP or network error for the log.
/// It is not an HrisException: it is an infrastructure failure, never a message for the API client.
/// </summary>
public class EmailDeliveryException : Exception
{
    public EmailDeliveryException() : base("The email could not be delivered.") { }
    public EmailDeliveryException(string message) : base(message) { }
    public EmailDeliveryException(string message, Exception innerException) : base(message, innerException) { }
}