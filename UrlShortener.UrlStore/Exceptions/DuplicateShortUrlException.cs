namespace UrlShortener.UrlStore.Exceptions;

/// <summary>
/// Exception that is thrown when a duplicate short URL is found.
/// </summary>  
public class DuplicateShortUrlException : Exception
{
    public DuplicateShortUrlException(string message) : base(message) { }
}

