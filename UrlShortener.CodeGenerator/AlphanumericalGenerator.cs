using System.Security.Cryptography;

namespace UrlShortener.CodeGenerator;

public class AlphanumericalGenerator : ICodeGenerator
{
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int MIN_LENGTH = 5;

    private static readonly RandomNumberGenerator random = RandomNumberGenerator.Create();
    
    private readonly int length;

    public AlphanumericalGenerator(int length)
    {
        if (length < 5)
            throw new ArgumentOutOfRangeException(nameof(MIN_LENGTH), $"Length must be at least {MIN_LENGTH}.");

        this.length = length;
    }

    public string GenerateCode()
    {
        int allowedCharsLenght = ALLOWED_CHARS.Length;

        return string.Concat(Enumerable.Repeat(ALLOWED_CHARS, length)
            .Select(c => c.ElementAt(RandomNumberGenerator.GetInt32(0, allowedCharsLenght))));
    }
}
