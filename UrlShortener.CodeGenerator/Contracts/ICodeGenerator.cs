namespace UrlShortener.CodeGenerator;

public interface ICodeGenerator
{
    /// <summary>
    /// Generates a code of a given length. 
    /// </summary>
    /// <returns>A string representing the generated code.</returns>
    string GenerateCode();
}
