using System.Security.Cryptography;

namespace UrlShortener.CodeGenerator;

public class AlphanumericalGenerator : ICodeGenerator
{
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int MIN_LENGTH = 5;

    private static readonly Random random = new Random();
    
    private readonly int length;

    public AlphanumericalGenerator(int length)
    {
        if (length < 5)
            throw new ArgumentOutOfRangeException(nameof(MIN_LENGTH), $"Length must be at least {MIN_LENGTH}.");

        this.length = length;
    }

    public string GenerateCode()
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = ALLOWED_CHARS[random.Next(ALLOWED_CHARS.Length)];
        }
        return new string(chars);
    }
}
