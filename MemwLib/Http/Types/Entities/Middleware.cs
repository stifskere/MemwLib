using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Entities;

/// <summary>Tells the server to execute the next middleware piece.</summary>
[PublicAPI]
public sealed class Middleware : IResponsible
{
    /// <summary>
    /// Creates a new instance of "Middleware" which lets you
    /// specify options to pass to the next handler.
    /// </summary>
    public static Middleware Next => new();
    
    internal Middleware() {}
    
    /// <inheritdoc cref="IResponsible.Headers"/>
    public HeaderCollection Headers { get; set; } = new();
    
    /// <inheritdoc cref="IResponsible.WithHeader"/>
    public Middleware WithHeader(string key, string value)
    {
        Headers.Add(key, value);
        return this;
    }

    IResponsible IResponsible.WithHeader(string key, string value) 
        => WithHeader(key, value);

    /// <inheritdoc cref="IResponsible.WithHeaders(Dictionary{string, string})"/>
    public Middleware WithHeaders(Dictionary<string, string> headers)
    {
        Headers.Add(headers);
        return this;
    }

    IResponsible IResponsible.WithHeaders(Dictionary<string, string> headers)
        => WithHeaders(headers);

    /// <inheritdoc cref="IResponsible.WithHeaders(HeaderCollection)"/>
    public Middleware WithHeaders(HeaderCollection headers)
    {
        Headers.Add(headers);
        return this;
    }
    
    IResponsible IResponsible.WithHeaders(HeaderCollection headers)
        => WithHeaders(headers);
}