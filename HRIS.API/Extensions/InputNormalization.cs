namespace HRIS.API.Extensions;

public static class InputNormalization
{
    public static string NormalizeEmail(this string email) => email.Trim().ToLowerInvariant();

    public static string NormalizeCode(this string code) => code.Trim().ToUpperInvariant();
}