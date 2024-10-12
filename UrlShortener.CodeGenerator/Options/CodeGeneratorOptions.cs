using Microsoft.Extensions.Options;

namespace UrlShortener.CodeGenerator;

/// <summary>
/// Options for the code generator.
/// </summary>
public class CodeGeneratorOptions
{
    /// <summary>
    /// Configuration section for the code generator.
    /// </summary>
    public const string ConfigurationSection = "CodeGenerator";

    /// <summary>
    /// Minimum length of the code to generate.
    /// </summary>
    public int MinCodeLength { get; set; } = 5;

    /// <summary>
    /// Length of the code to generate.
    /// </summary>
    public int CodeLength { get; set; } = 7;
}