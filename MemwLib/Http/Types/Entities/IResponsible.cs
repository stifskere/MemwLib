using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Entities;

/// <summary>Defines that a type is a response within the HTTP server.</summary>
public interface IResponsible
{
    /// <summary>The header collection corresponding to this HTTP entity.</summary>
    public HeaderCollection Headers { get; set; }
    
    /// <summary>Add a header to the response.</summary>
    /// <param name="key">The key of the header.</param>
    /// <param name="value">The value of the header.</param>
    /// <returns>An instance of self to act as a constructor</returns>
    public IResponsible WithHeader(string key, string value);

    /// <summary>Adds multiple headers to the response.</summary>
    /// <param name="headers">A dictionary of key-value pairs that represent the header key and value.</param>
    /// <returns>An instance of self to act as a constructor.</returns>
    public IResponsible WithHeaders(Dictionary<string, string> headers);

    /// <summary>Adds multiple headers to the response.</summary>
    /// <param name="headers">A header collection to append to this one header collection.</param>
    /// <returns>An instance of self to act as a constructor.</returns>
    public IResponsible WithHeaders(HeaderCollection headers);
}