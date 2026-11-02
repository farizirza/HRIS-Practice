using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace HRIS.API.Services.Email;

public class FileEmailTemplateRenderer(IWebHostEnvironment env) : IEmailTemplateRenderer
{
    public async Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> values, CancellationToken ct = default)
    {
        var path = Path.Combine(env.ContentRootPath, "Templates", "Emails", $"{templateName}.html");
        var html = await File.ReadAllTextAsync(path, ct);

        // One pass over the template. Values come from users (a name can contain "<script>" or even "{EmployeeNumber}"),
        // so every value is encoded, and the text that was inserted is never scanned for placeholders again.
        return Regex.Replace(html, @"\{(\w+)\}", match =>
            values.TryGetValue(match.Groups[1].Value, out var value)
                ? HtmlEncoder.Default.Encode(value)
                : match.Value);   // an unknown placeholder stays as it is
    }
}