namespace HRIS.API.Services.Email;

public interface IEmailTemplateRenderer
{
    /// <summary>Reads Templates/Emails/{templateName}.html and replaces every {key} with the encoded value.</summary>
    Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> values, CancellationToken ct = default);
}