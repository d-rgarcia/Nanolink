using Microsoft.Extensions.Options;

namespace UrlShortener.CodeGenerator;

public class AlphanumericalGenerator : ICodeGenerator
{
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private readonly IOptionsMonitor<CodeGeneratorOptions> options;
    private static readonly Random random = new Random();

    public AlphanumericalGenerator(IOptionsMonitor<CodeGeneratorOptions> options)
    {
        this.options = options;
    }

    public string GenerateCode()
    {
        var length = options.CurrentValue.CodeLength;
        if (length < options.CurrentValue.MinCodeLength)
            throw new ArgumentOutOfRangeException($"Configuration error: code length cannot be less than {options.CurrentValue.MinCodeLength}");

        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = ALLOWED_CHARS[random.Next(ALLOWED_CHARS.Length)];
        }
        return new string(chars);
    }
}
