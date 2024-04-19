namespace Reapit.Packages.Versioning.Exceptions;

public class ApiDateVersioningException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="ApiDateVersioningException"/> class.</summary>
    /// <param name="message">The message associated with the exception.</param>
    private ApiDateVersioningException(string message)
        : base(message)
    {
    }
    
    /// <summary>
    /// Factory method to create an instance of <see cref="ApiDateVersioningException"/> representing a missing API version
    /// date header.
    /// </summary>
    /// <param name="headerKey">The key of the header property expected to contain the API version date information.</param>
    /// <returns>A new instance of <see cref="ApiDateVersioningException"/>.</returns>
    public static ApiDateVersioningException MissingApiVersionDate(string headerKey) 
        => new(GetInvalidVersionMessage(headerKey));
    
    /// <summary>
    /// Factory method to create an instance of <see cref="ApiDateVersioningException"/> representing an invalid API version
    /// date header.
    /// </summary>
    /// <param name="headerKey">The key of the header property expected to contain the API version date information.</param>
    /// <returns>A new instance of <see cref="ApiDateVersioningException"/>.</returns>
    public static ApiDateVersioningException InvalidApiVersionDate(string headerKey) 
        => new(GetInvalidVersionMessage(headerKey));
    
    /// <summary>
    /// Factory method to create an instance of <see cref="ApiDateVersioningException"/> representing a provided API version
    /// date header which is incompatible with the date reported by the endpoint.
    /// </summary>
    /// <param name="provided">The API version date value that was provided in the header.</param>
    /// <returns>A new instance of <see cref="ApiDateVersioningException"/>.</returns>
    public static ApiDateVersioningException UnmatchedApiVersionDate(string provided) => 
        new(GetUnmatchedVersionMessage(provided));
    
    internal static string GetUnmatchedVersionMessage(string provided) 
        => $"HTTP resource does not support the API version '{provided}'";
    
    internal static string GetInvalidVersionMessage(string headerKey) => 
        $"HTTP resource requires that requests are issued with a '{headerKey}' header containing a version date in the format 'yyyy-MM-dd'.";
}